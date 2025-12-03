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

## 3. Typography and Icons

Font: **Poppins** or **Inter**. These are modern Sans-Serif fonts, highly readable on screen and offering multiple weights (light, regular, medium, bold) for proper information hierarchy.

- **Headings (H1, H2)**: Poppins Bold (White - #FFFFFF)
- **Body Text**: Poppins Regular (White - #FFFFFF or Light Gray - #E5E5E5 for secondary info)
- **Buttons**: Poppins Medium (White - #FFFFFF on orange background)
- **Icons**: Use a consistent library like Lucide Icons or Feather Icons. They are lightweight, modern, and their outline style will blend well with the clean design.

## 4. Conceptual Mockups (Screen Descriptions)

### a. Main Navigation

A fixed navigation bar at the bottom of the screen (Tab Bar), ideal for mobile:

- **Filter** (Funnel icon): The main and default screen.
- **Library** (Books/game boxes icon): The complete list of games.
- **History** (Clock icon): The history of played games.
- **Profile** (User icon): Account and settings management.

The active tab icon will be in Bright Orange (#FCA311), others in White (#FFFFFF).

### b. Home / Filter Screen

This is the heart of the application.

- **Title**: "What game tonight?" large and in white.
- **Clear form fields**:
  - Number of players
  - Available time (in minutes)
  - Game type (a multiple selection field with clickable "chips")
- **Main action button**: A large bright orange (#FCA311) button at the bottom of the screen with the text "Find a game".

### c. Library Screen

- **Title**: "My Library"
- **Search bar**: Allows quickly finding a game in the collection
- **Game list**: Each game is presented as a card
  - Card background: A very slightly lighter shade of the main background to distinguish it
  - Content: Game name in White, and below the key info (player count, duration) in Light Gray
- **Add button**: A bright orange (#FCA311) "Floating Action Button" (FAB) at the bottom right with a "+" icon

### d. Results & Launch Screen

After filtering, the application displays the list of compatible games (as cards).

- At the top, a summary of applied filters
- At the bottom, a highly visible button: "🎉 Roll the dice!" or "Choose randomly", always in bright orange (#FCA311)

### e. Result Modal

When the application has chosen a game, a modal (pop-up) appears for a "reveal" effect:

- Semi-transparent background to pause the backdrop
- Game image (when the API is integrated)
- "And the chosen game is..."
- **GAME NAME** in very large text, in bright orange (#FCA311)
- Two buttons: "Record the game" and "Choose another game"

## 5. Key Components

- **Buttons**:
  - Primary: Orange background, white text, rounded corners
  - Secondary: Gray border, transparent background, white text

- **Cards**: Rounded corners, very subtle drop shadow for depth effect

- **Form Fields**: Very light Light Gray background, with Bright Orange border when selected

This design offers a solid, functional, and aesthetically pleasing foundation that can evolve with the addition of future features (wishlist, statistics, etc.) while maintaining a consistent identity.

## 6. Decisions and Clarifications (2025-10-18)

- Icons: Use Lucide Icons (preference confirmed by the Product Owner). Integration via the InfiniLore.Lucide NuGet package (Blazor components) — no more CDN or JS initialization.
- Tab Bar: 4 tabs confirmed — Filter, Library, History, Profile. The "Library" and "History" tabs should remain visible but disabled until implemented.
- Typography: Keep Poppins as the default font.
- Branding: No specific logo for now. Keep default icons/values (favicon/app icon) until further notice.


### 6.1 Lucide Icons — Component Usage

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
- Prefer lowercase/kebab-case icon names such as "filter", "library", "history", "user".
