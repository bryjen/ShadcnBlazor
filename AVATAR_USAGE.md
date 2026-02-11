# Avatar Component Usage

The avatar module is text-only for now.

## Basic Avatar

```razor
<Avatar Text="BM" />
```

## Sizes

```razor
<div class="flex items-center gap-4">
    <Avatar Size="Size.Xs" Text="XS" />
    <Avatar Size="Size.Sm" Text="SM" />
    <Avatar Size="Size.Md" Text="MD" />
    <Avatar Size="Size.Lg" Text="LG" />
</div>
```

## Custom Styling

```razor
<Avatar Text="BM" Class="text-primary font-bold" />
```

## Avatar Group

```razor
<AvatarGroup Size="Size.Md">
    <Avatar Size="Size.Md" Text="BM" />
    <Avatar Size="Size.Md" Text="SM" />
    <Avatar Size="Size.Md" Text="AL" />
</AvatarGroup>
```

## Avatar Group with Count

```razor
<AvatarGroup Size="Size.Md">
    <Avatar Size="Size.Md" Text="BM" />
    <Avatar Size="Size.Md" Text="SM" />
    <Avatar Size="Size.Md" Text="AL" />
    <Avatar Size="Size.Md" Text="+5" />
</AvatarGroup>
```

## Key Points

- `Avatar` supports `Text`, `Size`, and `Class`.
- `AvatarGroup` controls overlap spacing with its `Size`.
- Keep avatar and group sizes aligned for best visual spacing.
- Image support is intentionally removed for now.
