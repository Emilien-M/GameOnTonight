# Design for "Game on Tonight"

This design aims to create a strong visual identity and a smooth user experience for the application.

## 1. Philosophy and General Ambiance

The approach is modern, clean, and focused on efficiency. The user wants to choose a game quickly, so the interface must get straight to the point.

- **Mobile-First**: Designed primarily for mobile devices, the design will adapt elegantly to larger screens.

- **High Contrast**: The chosen palette offers excellent readability. We will use a dark theme as the base, which is immersive and pleasant for evening use ("Game Night").

- **Clarity and Hierarchy**: Each screen will have a clear objective, guiding the user through strategic use of colors and typography.

## 2. Color Palette Usage

Color distribution is key to structuring the interface and guiding the eye.

| Color | Hexadecimal | Primary Role | Usage Examples |
|-------|-------------|--------------|----------------|
| Night Blue | #14213D | Main Background | Application background, creating a sober and focused atmosphere. |
| Bright Orange | #FCA311 | Accent & Actions | Primary buttons ("Start search"), active icons, links, selections, FAB (Floating Action Button). This is the color that says "Act here". |
| White | #FFFFFF | Primary Text | Titles, paragraphs, and icons on the Night Blue background for maximum readability. |
| Light Gray | #E5E5E5 | Secondary Elements | Less important text (e.g., "30-45 min"), subtle borders, separators, form field backgrounds. |
| Black | #000000 | Contrast & Depth | Can be used for text on very light backgrounds (if a light mode is considered) or to create subtle shadows for depth effect on elements. |

### 2.1 MudBlazor Theme Configuration (Implemented)

The color palette is implemented in `MainLayout.razor` using MudBlazor's `MudTheme`:

```csharp
PaletteDark = new PaletteDark
{
    Primary = new MudColor(252, 163, 17, 255),      // #FCA311 - Orange accent
    Secondary = new MudColor(229, 229, 229, 255),   // #E5E5E5 - Light Gray
    Background = new MudColor(20, 33, 61, 255),     // #14213D - Night Blue
    Surface = new MudColor(39, 52, 79, 255),        // Slightly lighter surface (#27344F)
    TextPrimary = new MudColor(255, 255, 255, 255), // White
    TextSecondary = new MudColor(229, 229, 229, 255),
    Error = new MudColor(255, 107, 107, 255),       // #FF6B6B - Red for errors
    Success = new MudColor(38, 176, 80, 255),       // #26B050 - Green for success
    Warning = new MudColor(252, 163, 17, 255),      // Orange
    Info = new MudColor(33, 150, 243, 255)          // #2196F3 - Blue info
}
```

### 2.2 CSS Variables (app.css)

```css
:root {
    --color-bg: #14213D;
    --color-accent: #FCA311;
    --color-text: #FFFFFF;
    --color-muted: #E5E5E5;
    --tabbar-height: 64px;
}
```

## 3. Typography and Icons

### 3.1 Typography

Font: **Poppins** — self-hosted in `wwwroot/fonts/poppins.css` (no CDN dependency).

- **Headings (H1, H2)**: Poppins Bold (White - #FFFFFF)
  - H1: `font-weight: 700; font-size: 1.75rem`
  - H2: `font-weight: 700; font-size: 1.35rem`
- **Body Text**: Poppins Regular (White/Light Gray)
- **Buttons**: Poppins Medium (White on orange background)

### 3.2 Icons

Two icon systems are used:

1. **Lucide Icons** (via `InfiniLore.Lucide` NuGet package) — Used in the Tab Bar:
   - `funnel` — Filter tab
   - `library` — Library tab
   - `history` — History tab
   - `chart-pie` — Statistics tab
   - `user` — Profile tab

2. **Material Icons** (via MudBlazor) — Used throughout components:
   - `Icons.Material.Filled.Add` — FAB buttons
   - `Icons.Material.Filled.Edit` — Edit actions
   - `Icons.Material.Filled.Delete` — Delete actions
   - `Icons.Material.Filled.Search` — Search fields
   - `Icons.Material.Filled.Casino` — Dice/random suggestion
   - `Icons.Material.Filled.EmojiEvents` — Winner indicator
   - `Icons.Material.Filled.Star` — Ratings
   - And many more for specific UI elements

## 4. Screen Descriptions (Implemented)

### a. Main Navigation (Tab Bar)

Fixed navigation bar at the bottom (`NavMenu.razor`), with 5 tabs:

| Tab | Icon | Route | Label |
|-----|------|-------|-------|
| Filter | `funnel` | `/filter/findForm` | Filtrer |
| Library | `library` | `/library` | Ludothèque |
| History | `history` | `/history` | Historique |
| Statistics | `chart-pie` | `/statistics` | Stats |
| Profile | `user` | `/profile` or `/login` | Profil |

**Tab Bar Styles** (`NavMenu.razor.css`):
- Fixed position at bottom
- Semi-transparent background with blur effect: `rgba(20,33,61,0.95)` + `backdrop-filter: blur(8px)`
- Active tab: Orange (#FCA311), Inactive: White
- Height: 64px (`--tabbar-height`)

### b. Authentication Screens

#### Login (`/login`)
- MudCard container with padding
- Title: "Connexion" (H4)
- Form fields:
  - Email (MudTextField, Outlined variant)
  - Password (MudTextField, password type)
- Primary button: "Se connecter"
- Link to registration

#### Register (`/register`)
- Same layout as login
- Title: "Créer un compte"
- Fields: Email, Password (min 6 chars)
- Success message with redirect to login

### c. Filter Screen (`/filter/findForm`)

The heart of the application — vertically centered content.

- **Title**: "Quelle partie ce soir ?" (MudText Typo.h3, centered)
- **Form fields** (max-width: 500px):
  - Number of players (MudNumericField, min: 1, default: 2)
  - Available time in minutes (MudNumericField, min: 5, step: 5, default: 60)
  - Game type (MudChipSet with single selection, chips populated from user's games)
- **Action button**: "Trouver un jeu" (MudButton, Variant.Filled, Color.Primary, Size.Large)
- Loading state: "Recherche…" with disabled button
- Error display: MudAlert with Severity.Error

### d. Results Screen (`/filter/result`)

- **Title**: "Jeux compatibles" (H3)
- **Back button**: MudIconButton with KeyboardReturn icon
- **Game list**: MudCard for each matching game
  - Game name (H5)
  - Chips: player range, duration, game type
- **Roll the dice button**: "Lancer le dé !" (full-width, primary color)
  - Loading state with MudProgressCircular

### e. Suggestion Modal (`SuggestionModal.razor`)

MudDialog with reveal effect:

- **Title**: "Et le jeu choisi est..."
- **Content**:
  - Casino icon (MudIcon, Size.Large, Color.Primary)
  - Game name (H4, Color.Primary, bold)
  - Info chips: player range, duration, game type
- **Actions**:
  - "Enregistrer la partie" (Primary, Filled)
  - "Choisir un autre jeu" (Default, Outlined) — triggers re-roll

### f. Save Game Session Modal (`SaveGameSessionModal.razor`)

Comprehensive form to record a played game:

- **Game info**: Display game name
- **Fields**:
  - Date picker (MudDatePicker, max: today)
  - Player count (MudNumericField)
  - Rating (MudRating, 1-5 stars, optional)
  - Photo URL (MudTextField, optional)
  - Notes (MudTextField multiline, optional)
- **Players section**:
  - Add player button (PersonAdd icon)
  - Per player: Name, Score, Position, Winner checkbox
  - Delete player button

### g. Library Screen (`/library`)

- **Title**: "Ma Ludothèque" (H3)
- **Search bar**: MudTextField with Search adornment
- **FAB**: Orange button with "+" icon (top-right of search row)
- **Game list** (virtualized for performance):
  - MudCard per game
  - Game name (H5)
  - Info chips: player range, duration, game type
  - Action buttons: Edit (Secondary), Delete (Warning)
- **Empty state**: MudAlert info message

### h. Edit/Create Game (`/library/new`, `/library/edit/{id}`)

- **Title**: "Ajouter un jeu" or "Modifier un jeu"
- **Form fields** (Outlined variant):
  - Name (required)
  - Min/Max players (side by side)
  - Duration in minutes
  - Game type (MudAutocomplete with existing types + new entry support)
- **Actions**: "Enregistrer" (Primary), "Annuler" (Outlined)

### i. Confirm Delete Dialog (`ConfirmDeleteDialog.razor`)

- **Content**: Confirmation message with game name
- **Warning text**: "Cette action est irréversible."
- **Actions**: "Annuler", "Supprimer" (Color.Error, Filled)

### j. History Screen (`/history`)

- **Title**: "Historique des parties" (H3)
- **Loading**: MudProgressCircular centered
- **Empty state**: History icon + message
- **Session list** (MudList):
  - MudCard per session
  - Game name with rating stars (if rated)
  - Chips: Date (dd/MM/yyyy), player count
  - Notes (if any)
  - Players section with chips (winner highlighted in green with trophy icon)
  - Photo thumbnail (if URL provided)
  - Delete button

### k. Statistics Screen (`/statistics`)

- **Title**: "Statistiques" (H3)
- **Summary cards** (MudGrid 4 columns):
  - Total games (Casino icon)
  - Total sessions (History icon)
  - Unique players (People icon)
  - Average rating (Star icon)
- **Top Games Chart**: MudChart Bar chart
- **Monthly Activity Chart**: MudChart Line chart
- **Player Stats Table** (MudTable):
  - Columns: Player name, Games played, Wins (with chip), Win rate (progress bar)
- **Top Games Details**: Ranked list with avatars and ratings

### l. Profile Screen (`/profile`)

- **Title**: "Mon Profil" (H3)
- **Profile info**: JSON display of user data (debug/placeholder)
- **Logout button**: MudButton Outlined, Color.Error, Logout icon

## 5. Key Components (MudBlazor)

### 5.1 Buttons

| Type | MudBlazor | Usage |
|------|-----------|-------|
| Primary | `Variant.Filled`, `Color.Primary` | Main actions (Submit, Save) |
| Secondary | `Variant.Outlined`, `Color.Default` | Cancel, back actions |
| Danger | `Variant.Filled`, `Color.Error` | Delete confirmations |
| FAB | `MudFab` with `Color.Primary` | Add new item |
| Icon | `MudIconButton` | Edit, delete, navigation |

### 5.2 Cards

- `MudCard` with `MudCardContent`
- Surface color (#27344F) for distinction from background
- Used for: game items, session items, statistics panels

### 5.3 Form Fields

- `MudTextField` — Text inputs (Outlined variant)
- `MudNumericField` — Numeric inputs with min/max
- `MudDatePicker` — Date selection
- `MudAutocomplete` — Type selection with suggestions
- `MudChipSet` with `MudChip` — Multi/single selection
- `MudRating` — Star ratings

### 5.4 Dialogs

- `MudDialog` with `MudDialogProvider`
- `DialogParameters<T>` for typed parameters
- `DialogOptions` for configuration (CloseOnEscapeKey, MaxWidth, FullWidth)

### 5.5 Feedback

- `MudAlert` — Error/info messages
- `MudSnackbar` via `ISnackbar` — Toast notifications
- `MudProgressCircular` — Loading spinners
- `MudProgressLinear` — Progress bars (e.g., win rates)

### 5.6 Data Display

- `MudTable` — Tabular data with sorting
- `MudList` / `MudListItem` — List displays
- `MudChart` — Bar and Line charts
- `Virtualize` — Performance optimization for long lists

## 6. Decisions and Clarifications

### 6.1 Original Decisions (2025-10-18)

- Icons: Use Lucide Icons (preference confirmed by the Product Owner). Integration via the InfiniLore.Lucide NuGet package (Blazor components) — no more CDN or JS initialization.
- Tab Bar: Originally 4 tabs — Filter, Library, History, Profile.
- Typography: Keep Poppins as the default font.
- Branding: No specific logo for now. Keep default icons/values (favicon/app icon) until further notice.

### 6.2 Implementation Updates (2025-12)

- **Tab Bar expanded to 5 tabs**: Added Statistics tab with `chart-pie` icon
- **All tabs now functional**: Filter, Library, History, Statistics, and Profile are fully implemented
- **MudBlazor integration**: Complete UI framework adoption with custom dark theme
- **Hybrid icon approach**: Lucide for tab bar, Material Icons (via MudBlazor) for component icons
- **French localization**: All UI text in French
- **PWA support**: Service worker, manifest, and icons configured

### 6.3 Lucide Icons — Component Usage

Use the Blazor component provided by the InfiniLore.Lucide NuGet package.

Minimal:

```razor
<LucideIcon Name="signature" />
```

With options:

```razor
<LucideIcon Name="arrow-right"
           Width="48"
           Height="48"
           Fill="none"
           Stroke="black"
           StrokeWidth="2"
           StrokeLineCap="round"
           StrokeLineJoin="round" />
```

Notes:
- Icons inherit the text color by default (currentColor). No CSS override is needed for the tab bar.
- Prefer lowercase/kebab-case icon names such as "funnel", "library", "history", "chart-pie", "user".

## 7. File Structure (Blazor App)

```
GameOnTonight.App/
├── App.razor                    # Router configuration
├── Program.cs                   # DI and service registration
├── _Imports.razor               # Global using statements
├── Layout/
│   ├── MainLayout.razor         # Main layout with MudBlazor providers and theme
│   ├── MainLayout.razor.css     # Layout styles
│   ├── NavMenu.razor            # Bottom tab bar
│   └── NavMenu.razor.css        # Tab bar styles
├── Pages/
│   ├── Login.razor              # Authentication
│   ├── Register.razor           # Registration
│   ├── Profile.razor            # User profile
│   ├── Filter/
│   │   ├── FindForm.razor       # Main filter form
│   │   ├── Result.razor         # Filter results
│   │   └── SuggestionModal.razor # Random game suggestion
│   ├── Library/
│   │   ├── Library.razor        # Game collection list
│   │   ├── EditBoardGame.razor  # Create/edit game form
│   │   ├── EditBoardGame.razor.cs # Code-behind
│   │   └── ConfirmDeleteDialog.razor
│   ├── History/
│   │   ├── History.razor        # Game session history
│   │   └── SaveGameSessionModal.razor # Record new session
│   └── Statistics/
│       └── Statistics.razor     # Stats dashboard
├── Services/                    # API service wrappers
└── wwwroot/
    ├── css/app.css              # Global styles and CSS variables
    ├── fonts/poppins.css        # Self-hosted Poppins font
    ├── index.html               # HTML entry point
    └── manifest.webmanifest     # PWA manifest
```

## 8. Responsive Design

- **Container**: `MudContainer` with `MaxWidth.Medium` for content centering
- **Grid**: `MudGrid` with responsive `xs`, `sm` breakpoints
- **Flexbox**: Heavy use of `d-flex`, `flex-column`, `flex-wrap` utilities
- **Mobile padding**: `pb-tabbar` class ensures content doesn't hide behind fixed tab bar
