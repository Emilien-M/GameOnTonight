# Design pour "Game on Tonight"

Ce design a pour but de créer une identité visuelle forte et une expérience utilisateur fluide pour l'application.

## 1. Philosophie et Ambiance Générale

L'approche est moderne, épurée et centrée sur l'efficacité. L'utilisateur veut choisir un jeu rapidement, l'interface doit donc aller droit au but.

- **Mobile-First** : Conçu d'abord pour les appareils mobiles (PWA), le design s'adaptera élégamment aux écrans plus grands.

- **Haut Contraste** : La palette choisie offre une excellente lisibilité. Nous utiliserons un thème sombre comme base, ce qui est immersif et agréable pour une utilisation en soirée ("Game Night").

- **Clarté et Hiérarchie** : Chaque écran aura un objectif clair, guidant l'utilisateur grâce à l'utilisation stratégique des couleurs et de la typographie.

## 2. Utilisation de la Palette de Couleurs

La distribution des couleurs est la clé pour structurer l'interface et guider le regard.

| Couleur | Hexadécimal | Rôle Principal | Exemples d'utilisation |
|---------|-------------|----------------|------------------------|
| Bleu Nuit | #14213D | Fond Principal | Arrière-plan de l'application, créant une ambiance sobre et concentrée. |
| Orange Vif | #FCA311 | Accentuation & Actions | Boutons principaux ("Lancer la recherche"), icônes actives, liens, sélections, FAB (Floating Action Button). C'est la couleur qui dit "Agissez ici". |
| Blanc | #FFFFFF | Texte Principal | Titres, paragraphes, et icônes sur le fond Bleu Nuit pour une lisibilité maximale. |
| Gris Clair | #E5E5E5 | Éléments Secondaires | Texte moins important (ex: "30-45 min"), bordures subtiles, séparateurs, arrière-plan des champs de formulaire. |
| Noir | #000000 | Contraste & Profondeur | Peut être utilisé pour les textes sur des fonds très clairs (si un mode clair est envisagé) ou pour créer des ombres subtiles pour donner de la profondeur aux éléments. |

## 3. Typographie et Icônes

Police de caractères : **Poppins** ou **Inter**. Ce sont des polices Sans-Serif modernes, très lisibles sur écran et offrant plusieurs graisses (light, regular, medium, bold) pour bien hiérarchiser l'information.

- **Titres (H1, H2)** : Poppins Bold (Blanc - #FFFFFF)
- **Texte de corps** : Poppins Regular (Blanc - #FFFFFF ou Gris Clair - #E5E5E5 pour les infos secondaires)
- **Boutons** : Poppins Medium (Blanc - #FFFFFF sur fond orange)
- **Icônes** : Utiliser une librairie cohérente comme Lucide Icons ou Feather Icons. Elles sont légères, modernes et leur style filaire se mariera bien avec le design épuré.

## 4. Maquettes Conceptuelles (Description par Écran)

### a. Navigation Principale

Une barre de navigation fixe en bas de l'écran (Tab Bar), idéale pour le mobile :

- **Filtrer** (Icône d'entonnoir) : L'écran principal et par défaut.
- **Ludothèque** (Icône de livres/boîtes de jeu) : La liste complète des jeux.
- **Historique** (Icône d'horloge) : L'historique des parties jouées.
- **Profil** (Icône d'utilisateur) : Gestion du compte et des paramètres.

L'icône de l'onglet actif sera en Orange Vif (#FCA311), les autres en Blanc (#FFFFFF).

### b. Écran d'Accueil / Filtrage

C'est le cœur de l'application.

- **Titre** : "Quelle partie ce soir ?" en grand et en blanc.
- **Champs de formulaire clairs** :
  - Nombre de joueurs
  - Temps disponible (en minutes)
  - Type de jeu (un champ de sélection multiple avec des "puces" cliquables)
- **Bouton d'action principal** : Un grand bouton orange vif (#FCA311) en bas de l'écran avec le texte "Trouver un jeu".

### c. Écran Ludothèque

- **Titre** : "Ma Ludothèque"
- **Barre de recherche** : Permet de trouver rapidement un jeu dans la collection
- **Liste de jeux** : Chaque jeu est présenté sous forme de carte
  - Fond de la carte : Une nuance très légèrement plus claire du fond principal pour la distinguer
  - Contenu : Nom du jeu en Blanc, et en dessous les infos clés (nb. joueurs, durée) en Gris Clair
- **Bouton d'ajout** : Un "Floating Action Button" (FAB) orange vif (#FCA311) en bas à droite avec une icône "+"

### d. Écran de Résultat & Lancement

Après avoir filtré, l'application affiche la liste des jeux compatibles (sous forme de cartes).

- En haut, un résumé des filtres appliqués
- En bas, un bouton très visible : "🎉 Lancer le dé !" ou "Choisir au hasard", toujours en orange vif (#FCA311)

### e. Modale de Résultat

Lorsque l'application a choisi un jeu, une modale (pop-up) apparaît pour un effet "révélation" :

- Fond semi-transparent pour mettre en pause l'arrière-plan
- Image du jeu (lorsque l'API sera intégrée)
- "Et le jeu choisi est..."
- **NOM DU JEU** en très grand, en orange vif (#FCA311)
- Deux boutons : "Enregistrer la partie" et "Choisir un autre jeu"

## 5. Composants Clés

- **Boutons** :
  - Primaire : Fond orange, texte blanc, coins arrondis
  - Secondaire : Bordure grise, fond transparent, texte blanc

- **Cartes (Cards)** : Coins arrondis, ombre portée très subtile pour un effet de profondeur

- **Champs de formulaire** : Arrière-plan Gris Clair très léger, avec une bordure Orange Vif lorsqu'il est sélectionné

Ce design offre une base solide, fonctionnelle et esthétiquement plaisante qui pourra évoluer avec l'ajout des fonctionnalités futures (wishlist, statistiques, etc.) tout en gardant une identité cohérente.