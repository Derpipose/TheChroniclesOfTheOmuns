# PlayerAppReact UI Conventions

Shared UI building blocks for the player app. Before writing a long run of Tailwind utilities or an inline `style={{ ... }}` object, check whether one of these already covers it.

All shared classes live in [src/index.css](src/index.css) and are built with Tailwind's `@apply`. Colors come from the theme variables in [src/css/theme.css](src/css/theme.css) (`--color-background`, `--color-text`, `--color-primary-30`, `--color-accent`, and so on).

## Rules of thumb

- **Colors belong in the class, not in an inline `style`.** Reach for `style` only for values that are truly dynamic (for example a progress-bar width). For a two-state color, switch between classes (`isActive ? 'pill-active' : ''`) instead of building a style object.
- **Unlayered CSS beats Tailwind utilities.** Rules in `index.css` are not inside a Tailwind `@layer`, so a utility cannot override a property the class already sets. Keep component classes free of properties you expect callers to change (this is why `.btn` sets no horizontal padding or width).
- **Pair component classes with layout utilities** (`flex-1`, `mt-2`, `px-4`, `w-full`) for things that vary per use.

## Layout and navigation

| Class | Purpose |
|---|---|
| `app-header` | Sticky top bar: `sticky top-0 z-30`, border, responsive padding, flex row. Used by `NavBar` and the page header bars on the character pages. Override the background inline only if a page needs a different shade. |
| `nav-btn` | Button inside the nav bar. Includes sizing, and tints toward the accent color on hover and focus. |
| `dice-page` | Dice roller root: `h-screen` flex column so the page itself never scrolls. |
| `dice-toolbar` | Roll mode and Clear Log bar. `shrink-0`, sits under the nav bar. |
| `dice-main` | Remaining height below the toolbar. A single scroller on mobile; fixed row with independently scrolling panels at `md+`. |
| `dice-inputs` | Left panel (roll inputs). `order-2` on mobile, one third wide at `md+`. |
| `roll-log` | Right panel (roll history). `order-1` on mobile, two thirds wide at `md+`. |

The dice page avoids double scrollbars by making the page a fixed-height flex column and giving only the panels `overflow-y-auto` at `md+`. Keep panels `shrink-0` on mobile, otherwise flex will collapse them.

## Forms and buttons

| Class | Purpose |
|---|---|
| `field-input` | Text input, number input, textarea or select: rounded, bordered, focus border, theme colors included. Add `w-full`, `w-16`, `font-mono` or `text-left` as needed. Add `panel` for the slightly lighter background. |
| `btn` | Base button: rounded, `py-2`, semibold, hover and active opacity, disabled state. Sets no color and no horizontal padding, so add `px-*`, `w-full` or `flex-1` at the use site. |
| `btn-accent` | Accent background with the on-accent text color. Use as `btn btn-accent`. |
| `btn-neutral` | Soft background with normal text. Add `border border-subtle` when it needs an outline. |
| `btn-danger` | Destructive action (secondary color tint). Add `border`. |
| `btn-alert` | Solid secondary color, for a warning state such as "reload when out of ammo". |
| `btn-sm` | Compact button for roll actions. Combine with `btn-accent` or `btn-neutral`, plus `flex-1`, `w-full` or `shrink-0`. Not combined with `btn`. |
| `choice-btn`, `choice-btn-active` | Selectable option card (theme picker). Add `-active` when selected. |
| `tab-btn`, `tab-btn-active` | Segmented tab switcher button. |
| `mode-chip`, `mode-chip-active` | Radio-style label chip (roll mode). |
| `form-section` | Character form column: plain stack on mobile, a bordered rounded card at `lg`. |
| `section-title` | Section heading inside the character form. Desktop only (`hidden lg:block`). |
| `collapse-summary` | The `<summary>` row for `<details>` blocks. |

## Surfaces, text and feedback

| Class | Purpose |
|---|---|
| `page` | Page root: `min-h-screen` on `--color-background`. Add `flex items-center justify-center` for a centered message. |
| `page-title` | Page heading (`text-xl`, semibold). |
| `card` | Rounded bordered panel on `--color-background-60`. Prefer the `Card` component. |
| `card-title` | Small uppercase heading for a card. |
| `panel` | Border and background colors for a lighter bordered surface (`--color-background-30`). Pair with `rounded-*`, `border` and padding. |
| `panel-danger` | Same slot as `panel` but tinted for an error state (for example a weapon that is out of ammo). |
| `label` | Small uppercase caption. |
| `empty-state` | Centered muted message for loading, error and empty lists. |
| `pill`, `pill-accent`, `pill-active` | Small fully rounded label. Prefer the `Pill` component. |
| `stat-tile`, `stat-tile-label`, `stat-tile-value` | Boxed label-and-value tile (AC, DR, HP, ability scores). |
| `row-chip` | Label on the left, value on the right, on a tinted background. |
| `avatar` | Square initial badge. |
| `progress`, `progress-fill` | Progress bar. Set only the fill `width` inline. |
| `status-dot`, `status-dot-active` | Small indicator dot. |
| `roll-attempt`, `roll-attempt-chosen` | Roll attempt box in the roll log. |
| `toast` | Fixed floating notice. Add inset utilities for position. |
| `alert-error`, `alert-success` | Inline form messages. |
| `fg`, `fg-muted`, `fg-accent`, `fg-danger`, `fg-success` | Text colors. |
| `bg-page`, `bg-soft`, `bg-tint` | Background colors (`--color-background`, `--color-background-60`, `--color-primary-30`). |
| `border-subtle` | Border color (`--color-primary-30`). Pair with `border` or `border-t`. |

## Shared components

| Component | Props | Notes |
|---|---|---|
| [Card](src/components/Card.tsx) | `title?`, `description?`, `className?` | Wraps `card` and `card-title`. Add layout utilities (`flex flex-col gap-4`) through `className`. |
| [Pill](src/components/Pill.tsx) | `variant?: 'neutral' \| 'accent' \| 'active'` | Wraps the `pill` classes. |

Character view pieces live in [src/components/CharacterViewPage/](src/components/CharacterViewPage/): `OverviewCard`, `DetailsCard` and `StatListCard` (mobile), `HeroCard`, `VitalsCard`, `AbilityScoresCard` and `TraitCards` (desktop), plus `StatTile`, `DetailRow`, `Expandable`, `StatusPills` and `CenteredMessage`. [CharacterViewPage.tsx](src/pages/CharacterViewPage.tsx) fetches the data and composes them.

Dev page pieces live in [src/components/DevPage/](src/components/DevPage/): `SeedButton` (seed row with status), `Field` (label wrapping a control), `EnumSelect` (select generated from a const-object enum), `AddWeaponCard` and `DeleteWeaponCard` (each owns its own state and mutations). [DevPage.tsx](src/pages/DevPage.tsx) only composes them.

## Known exceptions and gaps

- [SignInPage.tsx](src/pages/SignInPage.tsx) intentionally keeps its own hard-coded palette (it renders before a theme is loaded), so it does not use the themed classes above. It does use `btn`.
- Page layout wrappers such as `max-w-md mx-auto px-6 py-8 ...` are still written out per page.
- Some hard-coded hex colors (`#4A7C59`, `#4C8B4C`, `#E4EEE4`, `#3D5C3D`, `#C9D9C9`, `#5C8A5C`) now live only in `index.css` and should become theme variables.
- Several pages mix `rounded-2xl border p-6 ... panel` for a card on `--color-background-30`. If these should match `card`, switch them to `Card`.
