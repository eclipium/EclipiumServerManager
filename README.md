# EclipiumServerManager
## Un gestionnaire d'applications utilisant systemd
Il vous permettera de gérer des applications NodeJS, Python, Java, .NET 5/6/Core 3.1, et Go sous les distributions Linux utilisant systemd.

## Systèmes supportés
- Ubuntu 18.04, 20.04
- Debian 9, 10, 11
- Almalinux 8.5+, CentOS 8.5+, RHEL 8.5+...
- Fedora 36+

## Liste des commandes

```
esm list => Affiche la liste des applications hébergées
esm status <app> => Affiche le statut d'une application
esm log <app> => Affiche l'historique de la console de l'application
esm start <app> => Démarre une application
esm stop <app> => Arrête une application
esm restart <app> => Redémmare une application
esm kill <app> => Tue le processus d'une application
esm enable <app> => Active le démarrage automatique de l'application au démarrage du système
esm disable <app> => Désactive le démarrage automatique de l'application au démarrage du système
esm delete <app> => Supprime l'application
esm rename <app> <new-name> => Renomme l'application
esm backup <app> => Crée une sauvegarde de l'application
esm backup all => Crée une sauvegarde de toutes les applications
esm list-backups => Liste toutes les sauvegardes
esm list-backups <app> => Liste les sauvegardes d'une application
esm delete-backup <backup-name> => Supprime une backup si elle existe
esm restore-backup <backup-name> => Restaure une backup si elle existe

```

## Installation des dépendances pour Ubuntu/Debian

ESM nécessite l'installation du runtime .NET 6

**Ubuntu 20.04 :**

``wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb``

**Ubuntu 18.04 :**

``wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb``

**Debian 11 :**

``wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb``

**Debian 10 :**

``wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb``

**Debian 9 :**

``wget https://packages.microsoft.com/config/debian/9/packages-microsoft-prod.deb -O packages-microsoft-prod.deb``

Le reste des commandes est commun a Ubuntu et Debian toutes versions confondues

```
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
apt-get update
apt-get install -y apt-transport-https
apt-get update
apt-get install -y dotnet-runtime-6.0
```
