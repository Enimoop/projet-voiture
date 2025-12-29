## 1. Introduction

Ce projet consiste à concevoir un système de gestion de location de véhicules en C#, respectant un ensemble de contraintes métier (durée maximale, gestion des clients, options, promotions, maintenance, etc.).


## 2. Démarche générale

La démarche adoptée a été la suivante :

- Analyse des besoins fonctionnels et des contraintes métier

- Identification des entités principales du domaine

- Définition des responsabilités de chaque classe

- Mise en place des relations entre les classes

- Implémentation progressive des fonctionnalités

- Ajout d’un menu console pour simuler l’utilisation du système


## 3. Choix de conception
### 3.1 Classe Client (abstraite)

La classe Client représente un client du système.
Elle est définie comme abstraite afin de permettre plusieurs types de clients.

Rôles principaux :

- Stocker les informations du client

- Gérer ses locations actives et son historique

- Définir les règles communes (remise longue durée, limite de locations)

Client\
 ├── Particulier\
 └── Premium

<br>

Justification :
- Le polymorphisme permet de gérer facilement des comportements différents selon le type de client.

- La méthode abstraite GetDepotGarantie() impose à chaque type de client de définir son dépôt.

### 3.2 Classes Particulier et Premium

Ces classes héritent de Client.

| Type client | Dépôt de garantie | Remise                     |
| ----------- | ----------------- | -------------------------- |
| Particulier | 300 €             | 15 % à partir de 7 jours   |
| Premium     | 150 €             | 15 % + 10 % supplémentaire |

<br>


Justification :

- Le client Premium bénéficie d’avantages supplémentaires (dépôt réduit, remise plus importante).

- Cela permet de montrer l’utilisation de l’héritage et de la redéfinition de méthodes (override).

### 3.3 Classe Vehicle

La classe Vehicle représente un véhicule de la flotte.

Responsabilités :

- Stocker les caractéristiques du véhicule

- Gérer son état (Available, Rented, Maintenance)

- Conserver l’historique des locations

- Vérifier la disponibilité sur une période donnée

- Gérer la maintenance (10 000 km ou 6 mois)

<br>


Justification :

- La méthode IsAvailable(start, end) garantit qu’un véhicule ne peut être loué que par un seul client à la fois.

- La maintenance est intégrée directement dans le véhicule pour rester proche de la réalité métier.

### 3.4 Classe Rental

La classe Rental est centrale : elle relie un client à un véhicule sur une période donnée.

Responsabilités :

- Vérifier les contraintes métier à la création :

    - durée maximale (30 jours)

    - limite de 3 locations simultanées par client

    - disponibilité du véhicule

- Gérer les options, promotions et remises

- Générer la facture

- Gérer la fin de location, l’annulation et la résiliation anticipée

- Enregistrer l’inspection et les kilomètres parcourus


<br>


Justification :

- Toute la logique métier liée à une location est regroupée dans une seule classe.

- Cela évite de disperser les règles dans le programme principal.

3.5 Classe Option

Les options (GPS, siège enfant, assurance) sont modélisées via une classe dédiée.

Types d’options :

- Journalier : prix × nombre de jours

- Fixe : prix unique


<br>


Justification :

- Permet de respecter précisément l’énoncé.

- Facilite l’ajout de nouvelles options sans modifier la classe Rental.

### 3.6 Classe Promotion

La classe Promotion permet d’appliquer une réduction supplémentaire sur le prix final.

Justification :

- Séparation claire entre la logique de promotion et celle de la location.

- Permet de gérer plusieurs promotions sans complexifier le calcul du prix.

### 3.7 Classe Facture

La facture est générée à la fin de la location, après inspection du véhicule.

Contenu :

- Identifiant de la location

- Date

- Montant total

- Dépôt de garantie

<br>


Justification :

- Le montant final n’est connu qu’à la restitution du véhicule.

- La facture est stockée dans l’historique du client.

## 4. Gestion des contraintes métier
| Contrainte                   | Implémentation                     |
| ---------------------------- | ---------------------------------- |
| 1 seul client par véhicule   | `Vehicle.IsAvailable()`            |
| Prix dépend du véhicule      | `BasicPrice`                       |
| Options journalières / fixes | `OptionType`                       |
| Max 3 locations client       | `Client.PeutLouer()`               |
| Max 30 jours                 | constructeur `Rental`              |
| Remise ≥ 7 jours             | `Client.CalculerRemise()`          |
| Promotions                   | `Promotion`                        |
| Dépôt de garantie            | `GetDepotGarantie()`               |
| Inspection retour            | paramètres de `TerminerLocation()` |
| Maintenance                  | 10 000 km ou 6 mois                |

## 5. Interface utilisateur (console)

Un menu console permet :

- l’enregistrement d’un client

- la création de locations

- la gestion des options et promotions

- la résiliation ou l’annulation

- l’affichage de l’historique et des factures

## 6. Conclusion

Les choix de conception reposent sur une séparation claire des responsabilités entre les classes.
L’héritage et le polymorphisme permettent de gérer différents types de clients et leurs avantages.
La classe Rental centralise les règles métier (prix, durée, options, promotions, facturation), tandis que Vehicle gère la disponibilité et la maintenance.
