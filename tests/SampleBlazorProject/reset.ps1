# This script resets the SampleBlazorProject directory to a specific commit state.
# Notably, one where no changes to the project files were made.
# Allows for testing functionality for a "tracked" project without any actual changes.
git checkout abdbac3 -- tests/SampleBlazorProject && git clean -fd tests/SampleBlazorProject