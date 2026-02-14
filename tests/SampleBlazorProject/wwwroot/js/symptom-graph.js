// D3.js Force-Directed Graph for Symptom Correlation Map
// Converted from React component to vanilla JS for Blazor interop

const graphInstances = new Map();

function getContainer(elementId) {
    return document.getElementById(elementId);
}

function updateDimensions(elementId) {
    const container = getContainer(elementId);
    if (!container) return { width: 0, height: 0 };
    const rect = container.getBoundingClientRect();
    return { width: rect.width, height: rect.height };
}

// Store DotNetObjectReference for callbacks
if (!window.dotNetRefs) {
    window.dotNetRefs = {};
}

export function initializeGraph(elementId, data, dotNetRef) {
    console.log('Initializing graph for element:', elementId, 'DotNetRef:', !!dotNetRef);
    // Store DotNetObjectReference for this graph instance
    if (dotNetRef) {
        window.dotNetRefs[elementId] = dotNetRef;
        console.log('DotNet reference stored for:', elementId);
    } else {
        console.warn('No DotNet reference provided for:', elementId);
    }
    const container = getContainer(elementId);
    if (!container || !data || !window.d3) {
        console.error('Graph initialization failed: container, data, or d3 not available', {
            container: !!container,
            data: !!data,
            d3: !!window.d3
        });
        return;
    }

    // Parse JSON data if it's a string
    let graphData = data;
    if (typeof data === 'string') {
        try {
            graphData = JSON.parse(data);
        } catch (e) {
            console.error('Failed to parse graph data:', e);
            return;
        }
    }

    // Clean up existing instance if any
    if (graphInstances.has(elementId)) {
        disposeGraph(elementId);
    }

    const dimensions = updateDimensions(elementId);
    if (dimensions.width === 0 || dimensions.height === 0) {
        console.warn('Container has zero dimensions, retrying after delay');
        setTimeout(() => initializeGraph(elementId, data), 100);
        return;
    }

    const { width, height } = dimensions;
    const d3 = window.d3;

    // Get CSS color variables from computed styles
    const rootStyle = getComputedStyle(document.documentElement);
    const getCSSVar = (varName) => rootStyle.getPropertyValue(varName).trim();
    
    // Helper to convert HSL/OKLCH to usable color
    const getColor = (varName, fallback) => {
        const value = getCSSVar(varName);
        if (!value) return fallback;
        // If it's already a hex or rgb, use it directly
        if (value.startsWith('#') || value.startsWith('rgb')) return value;
        // If it's hsl format, wrap it
        if (value.startsWith('hsl')) return value;
        // For oklch or other formats, try to use as-is or return fallback
        return value || fallback;
    };
    
    // Extract colors - use CSS variables with fallbacks
    const primaryColor = getColor('--color-primary-neon', '#5288ed');
    const mutedColor = getColor('--color-muted-foreground', '#9ca3af');
    const backgroundColor = getColor('--color-background-dark', '#0f172a');
    const foregroundColor = getColor('--color-foreground', '#ffffff');

    // Create SVG
    const svg = d3.select(container)
        .append('svg')
        .attr('width', width)
        .attr('height', height)
        .style('cursor', 'move');

    // Create container group for zoom
    const g = svg.append('g');

    // Zoom behavior
    const zoom = d3.zoom()
        .scaleExtent([0.1, 4])
        .on('zoom', (event) => {
            g.attr('transform', event.transform);
        });

    svg.call(zoom);
    
    // Add background rect first (behind everything) to catch clicks for deselection
    // Use pointer-events: none initially, we'll handle background clicks differently
    const backgroundRect = g.insert('rect', ':first-child')
        .attr('width', width)
        .attr('height', height)
        .attr('fill', 'transparent')
        .attr('pointer-events', 'none') // Don't block clicks to nodes
        .style('cursor', 'move');
    
    // Handle background clicks on the SVG itself instead
    svg.on('click', function(event) {
        // Only deselect if clicking on SVG background (not on nodes/links)
        const target = event.target;
        if (target === svg.node() || (target.tagName === 'rect' && target.getAttribute('fill') === 'transparent')) {
            console.log('Background clicked - deselecting');
            if (window.dotNetRefs && window.dotNetRefs[elementId]) {
                window.dotNetRefs[elementId].invokeMethodAsync('OnNodeClick', null);
            }
        }
    });

    // Clone data to avoid mutation
    const nodes = graphData.nodes.map(d => ({ ...d }));
    const links = graphData.links.map(d => ({ ...d }));

    // Force simulation
    const simulation = d3.forceSimulation(nodes)
        .force('link', d3.forceLink(links).id(d => d.id).distance(100))
        .force('charge', d3.forceManyBody().strength(-300))
        .force('center', d3.forceCenter(width / 2, height / 2))
        .force('collide', d3.forceCollide().radius(d => (d.value * 2) + 20));

    // Add defs for filters and gradients
    const defs = svg.append('defs');
    
    // Glow filter for root nodes - enhanced glow effect
    const filter = defs.append('filter')
        .attr('id', `glow-${elementId}`)
        .attr('x', '-50%')
        .attr('y', '-50%')
        .attr('width', '200%')
        .attr('height', '200%');
    
    // Create a colored glow using feColorMatrix and feGaussianBlur
    const feColorMatrix = filter.append('feColorMatrix')
        .attr('in', 'SourceGraphic')
        .attr('type', 'matrix')
        .attr('values', '0 0 0 0 0  0 0 0 0 0.5  0 0 0 0 1  0 0 0 1 0');
    
    filter.append('feGaussianBlur')
        .attr('stdDeviation', '4')
        .attr('result', 'coloredBlur');
    
    const feMerge = filter.append('feMerge');
    feMerge.append('feMergeNode').attr('in', 'coloredBlur');
    feMerge.append('feMergeNode').attr('in', 'SourceGraphic');
    
    // Radial gradient for cluster boundary - using theme colors
    const clusterGradient = defs.append('radialGradient')
        .attr('id', `cluster-gradient-${elementId}`)
        .attr('cx', '50%')
        .attr('cy', '50%')
        .attr('r', '50%');
    clusterGradient.append('stop')
        .attr('offset', '0%')
        .attr('stop-color', 'rgba(30, 80, 120, 0.3)');
    clusterGradient.append('stop')
        .attr('offset', '100%')
        .attr('stop-color', 'rgba(60, 150, 200, 0.1)');

    // Create cluster boundary group first (so it renders behind everything)
    const clusterGroup = g.append('g')
        .attr('class', 'cluster-boundary');
    
    // Draw Links
    const link = g.append('g')
        .attr('class', 'links')
        .selectAll('line')
        .data(links)
        .enter().append('line')
        .attr('class', 'link-line')
        .attr('stroke', d => {
            // High correlation (value >= 3): solid blue
            // Low correlation (value < 3): dashed gray
            return d.value >= 3 ? primaryColor : mutedColor;
        })
        .attr('stroke-opacity', d => d.value >= 3 ? 0.6 : 0.4)
        .attr('stroke-width', d => Math.sqrt(d.value))
        .attr('stroke-dasharray', d => d.value >= 3 ? 'none' : '4,4');

    // Draw Nodes
    console.log('Creating', nodes.length, 'nodes');
    const node = g.append('g')
        .attr('class', 'nodes')
        .selectAll('g')
        .data(nodes)
        .enter().append('g')
        .attr('cursor', 'pointer')
        .attr('class', d => `node-group node-${d.id}`)
        .on('click', function(event, d) {
            // Don't fire click if a drag just occurred
            if (dragOccurred) {
                console.log('Click ignored - drag occurred');
                return;
            }
            event.stopPropagation();
            console.log('Node clicked:', d.id, 'DotNetRef available:', !!window.dotNetRefs?.[elementId]);
            // Notify Blazor about node click
            if (window.dotNetRefs && window.dotNetRefs[elementId]) {
                window.dotNetRefs[elementId].invokeMethodAsync('OnNodeClick', d.id)
                    .catch(err => console.error('Error calling OnNodeClick:', err));
            } else {
                console.warn('DotNet reference not found for element:', elementId);
            }
        })
        .call(d3.drag()
            .on('start', dragstarted)
            .on('drag', dragged)
            .on('end', dragended));

    // Node Circles
    node.append('circle')
        .attr('class', 'node-circle')
        .attr('r', d => {
            if (d.type === 'root') return 30;
            if (d.type === 'diagnosis') return 15;
            return 8;
        })
        .attr('fill', d => {
            // Root: solid blue fill
            if (d.type === 'root') return primaryColor;
            // Diagnosis and Symptom: transparent (hollow)
            return 'transparent';
        })
        .attr('stroke', d => {
            // Root: no stroke (solid fill)
            if (d.type === 'root') return 'none';
            // Diagnosis: blue stroke
            if (d.type === 'diagnosis') return primaryColor;
            // Symptom: muted gray stroke
            return mutedColor;
        })
        .attr('stroke-width', d => {
            if (d.type === 'root') return 0;
            return 2;
        })
        .style('pointer-events', 'all') // Ensure circles receive clicks
        .on('click', function(event, d) {
            // Don't fire click if a drag just occurred
            if (dragOccurred) {
                console.log('Circle click ignored - drag occurred');
                return;
            }
            event.stopPropagation();
            console.log('Circle clicked:', d.id, 'DotNetRef available:', !!window.dotNetRefs?.[elementId]);
            // Notify Blazor about node click
            if (window.dotNetRefs && window.dotNetRefs[elementId]) {
                window.dotNetRefs[elementId].invokeMethodAsync('OnNodeClick', d.id)
                    .catch(err => console.error('Error calling OnNodeClick:', err));
            } else {
                console.warn('DotNet reference not found for element:', elementId);
            }
        });

    // Apply glow filter to root nodes
    node.filter(d => d.type === 'root')
        .style('filter', `url(#glow-${elementId})`);

    // Node Labels
    node.append('text')
        .text(d => d.label)
        .attr('x', d => d.type === 'root' ? 40 : 15)
        .attr('y', 5)
        .attr('fill', foregroundColor)
        .attr('font-size', d => d.type === 'root' ? '16px' : '12px')
        .attr('font-family', 'monospace')
        .attr('opacity', 0.9)
        .style('pointer-events', 'none')
        .style('text-shadow', '0 0 8px rgba(0, 0, 0, 0.8)');

    // Drag functions
    // Track if a drag occurred to prevent click after drag
    let dragOccurred = false;
    
    function dragstarted(event, d) {
        dragOccurred = false;
        if (!event.active) simulation.alphaTarget(0.3).restart();
        d.fx = d.x;
        d.fy = d.y;
    }

    function dragged(event, d) {
        dragOccurred = true; // Mark that a drag occurred
        d.fx = event.x;
        d.fy = event.y;
    }

    function dragended(event, d) {
        if (!event.active) simulation.alphaTarget(0);
        d.fx = null;
        d.fy = null;
        // Reset drag flag after a short delay to allow click handler to check it
        setTimeout(() => { dragOccurred = false; }, 100);
    }
    
    let clusterCircle = null;
    let clusterLabel = null;
    
    // Function to update cluster boundary based on node positions
    function updateClusterBoundary() {
        if (nodes.length === 0) return;
        
        // Calculate bounding box of all nodes
        let minX = Infinity, maxX = -Infinity;
        let minY = Infinity, maxY = -Infinity;
        
        nodes.forEach(d => {
            const radius = d.type === 'root' ? 30 : (d.type === 'diagnosis' ? 15 : 8);
            minX = Math.min(minX, d.x - radius);
            maxX = Math.max(maxX, d.x + radius);
            minY = Math.min(minY, d.y - radius);
            maxY = Math.max(maxY, d.y + radius);
        });
        
        // Calculate center and radius with padding
        const centerX = (minX + maxX) / 2;
        const centerY = (minY + maxY) / 2;
        const padding = 60; // Extra padding around nodes
        const radius = Math.max(
            Math.sqrt(Math.pow(maxX - minX, 2) + Math.pow(maxY - minY, 2)) / 2 + padding,
            150 // Minimum radius
        );
        
        // Remove existing cluster elements
        clusterGroup.selectAll('*').remove();
        
        // Get colors for cluster boundary (use primary-neon for cyan theme)
        const clusterBorderColor = primaryColor || '#3a8fb7';
        const clusterLabelColor = primaryColor || '#81e6d9';
        
        // Draw cluster circle with gradient fill
        clusterCircle = clusterGroup.append('circle')
            .attr('cx', centerX)
            .attr('cy', centerY)
            .attr('r', radius)
            .attr('fill', `url(#cluster-gradient-${elementId})`)
            .attr('stroke', clusterBorderColor)
            .attr('stroke-width', 1)
            .attr('stroke-dasharray', '4,4')
            .attr('stroke-opacity', 0.6)
            .style('pointer-events', 'none');
        
        // Add cluster label at the top with background highlight
        const labelBg = clusterGroup.append('rect')
            .attr('x', centerX - 80)
            .attr('y', centerY - radius - 20)
            .attr('width', 160)
            .attr('height', 14)
            .attr('rx', 2)
            .attr('fill', 'rgba(0, 0, 0, 0.5)')
            .style('pointer-events', 'none');
        
        clusterLabel = clusterGroup.append('text')
            .attr('x', centerX)
            .attr('y', centerY - radius - 10)
            .attr('text-anchor', 'middle')
            .attr('fill', clusterLabelColor)
            .attr('font-size', '10px')
            .attr('font-family', 'monospace')
            .attr('font-weight', 'bold')
            .attr('text-transform', 'uppercase')
            .attr('letter-spacing', '0.1em')
            .attr('opacity', 0.8)
            .style('pointer-events', 'none')
            .text('CLUSTER: INFLUENZA_A');
    }
    
    // Simulation tick handler
    simulation.on('tick', () => {
        link
            .attr('x1', d => d.source.x)
            .attr('y1', d => d.source.y)
            .attr('x2', d => d.target.x)
            .attr('y2', d => d.target.y);

        node
            .attr('transform', d => `translate(${d.x},${d.y})`);
        
        // Update cluster boundary on each tick
        updateClusterBoundary();
    });
    
    // Initial cluster boundary update (nodes may have initial positions)
    setTimeout(() => updateClusterBoundary(), 100);

    // Handle resize
    const resizeHandler = () => {
        const newDimensions = updateDimensions(elementId);
        if (newDimensions.width > 0 && newDimensions.height > 0) {
            svg.attr('width', newDimensions.width)
               .attr('height', newDimensions.height);
            // Update background rect size
            g.select('rect[fill="transparent"]')
                .attr('width', newDimensions.width)
                .attr('height', newDimensions.height);
            simulation.force('center', d3.forceCenter(newDimensions.width / 2, newDimensions.height / 2));
            simulation.alpha(0.3).restart();
        }
    };

    window.addEventListener('resize', resizeHandler);
    
    // Store instance data
    graphInstances.set(elementId, {
        svg,
        simulation,
        resizeHandler,
        container,
        primaryColor,
        mutedColor,
        nodes,
        links
    });
}

export function updateGraph(elementId, data) {
    // Parse JSON data if it's a string
    let graphData = data;
    if (typeof data === 'string') {
        try {
            graphData = JSON.parse(data);
        } catch (e) {
            console.error('Failed to parse graph data:', e);
            return;
        }
    }
    
    // Preserve the DotNet reference before disposing
    const dotNetRef = window.dotNetRefs && window.dotNetRefs[elementId] ? window.dotNetRefs[elementId] : null;
    
    // For now, just reinitialize with new data
    // Could be optimized to update nodes/links without full recreation
    disposeGraph(elementId);
    
    // Reinitialize with preserved DotNet reference
    initializeGraph(elementId, graphData, dotNetRef);
}

export function disposeGraph(elementId, cleanupDotNetRef = false) {
    const instance = graphInstances.get(elementId);
    if (instance) {
        // Stop simulation
        if (instance.simulation) {
            instance.simulation.stop();
        }

        // Remove resize listener
        if (instance.resizeHandler) {
            window.removeEventListener('resize', instance.resizeHandler);
        }

        // Remove SVG
        if (instance.container && window.d3) {
            window.d3.select(instance.container).select('svg').remove();
        }

        // Clean up DotNet reference only if explicitly requested (component disposal)
        // Otherwise preserve it for updateGraph
        if (cleanupDotNetRef && window.dotNetRefs) {
            delete window.dotNetRefs[elementId];
        }

        graphInstances.delete(elementId);
    }
}

export function setSelectedNode(elementId, nodeId) {
    const instance = graphInstances.get(elementId);
    if (!instance || !instance.svg) return;
    
    const d3 = window.d3;
    const svg = instance.svg;
    const primaryColor = instance.primaryColor || '#5288ed';
    const mutedColor = instance.mutedColor || '#9ca3af';
    const nodes = instance.nodes || [];
    const links = instance.links || [];
    
    // Update node visual state
    svg.selectAll('.node-group').each(function(d) {
        const nodeEl = d3.select(this);
        const isSelected = nodeId && d.id === nodeId;
        const circleEl = nodeEl.select('.node-circle');
        
        // Determine original values
        const originalStroke = d.type === 'root' ? 'none' : (d.type === 'diagnosis' ? primaryColor : mutedColor);
        const originalStrokeWidth = d.type === 'root' ? 0 : 2;
        
        // Stop any ongoing transitions to prevent visual artifacts
        circleEl.interrupt();
        
        // When deselecting (nodeId is null), explicitly reset all attributes immediately
        // to prevent rainbow effect
        if (!nodeId) {
            // Deselecting - set everything immediately to prevent artifacts
            circleEl
                .attr('stroke', originalStroke)
                .attr('stroke-width', originalStrokeWidth);
        } else {
            // Selecting - transition smoothly
            circleEl
                .transition()
                .duration(200)
                .attr('stroke', isSelected ? '#fbbf24' : originalStroke)
                .attr('stroke-width', isSelected ? 4 : originalStrokeWidth);
        }
    });
    
    // Update link visual state - highlight links connected to selected node
    svg.selectAll('.link-line').each(function(d) {
        const linkEl = d3.select(this);
        // Handle both object and string IDs
        const sourceId = typeof d.source === 'object' ? d.source.id : d.source;
        const targetId = typeof d.target === 'object' ? d.target.id : d.target;
        const isConnected = nodeId && (sourceId === nodeId || targetId === nodeId);
        
        // Determine original values
        const originalStroke = d.value >= 3 ? primaryColor : mutedColor;
        const originalOpacity = d.value >= 3 ? 0.6 : 0.4;
        const originalWidth = Math.sqrt(d.value);
        const originalDashArray = d.value >= 3 ? 'none' : '4,4';
        
        // Stop any ongoing transitions to prevent visual artifacts
        linkEl.interrupt();
        
        // When deselecting (nodeId is null), explicitly reset all attributes immediately
        // then transition smoothly to prevent rainbow effect
        if (!nodeId) {
            // Deselecting - set everything immediately to prevent artifacts
            linkEl
                .attr('stroke', originalStroke)
                .attr('stroke-opacity', originalOpacity)
                .attr('stroke-width', originalWidth)
                .attr('stroke-dasharray', originalDashArray);
        } else {
            // Selecting - set dash array immediately, transition the rest
            linkEl.attr('stroke-dasharray', isConnected ? 'none' : originalDashArray);
            
            // Transition only color, opacity, and width
            linkEl
                .transition()
                .duration(200)
                .attr('stroke', isConnected ? '#fbbf24' : originalStroke)
                .attr('stroke-opacity', isConnected ? 0.8 : originalOpacity)
                .attr('stroke-width', isConnected ? 3 : originalWidth);
        }
    });
}
