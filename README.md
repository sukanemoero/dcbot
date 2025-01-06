# The discord lottery bot using dotnet  
Just for class and fun. Maybe you can make some issues or PR, that can make this better.
Also I didn't send a request regarding copyright. If there are any issues, please feel free to raise an issue or DM me.

## Image source

https://github.com/SchaleDB/SchaleDB

## Character source

https://github.com/torikushiii/BlueArchiveAPI/

# Starup
This project run in NET9.0. [INSTALLATION](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
And the database using MySql or MariaDB [INSTALLATION](https://mariadb.org/download/)

## Build
If all done, let's start build this repo.

```bash
git clone https://github.com/sukanemoero/dcbot.git LotteryDiscordBot
cd LotteryDiscordBot
rm -rf .git .gitignore
mkdir -p config
echo "{host: \"\", user: \"\", password: \"\", database: \"\"}" > ./config/database_config.json
echo "{token:\"\" }" > ./config/bot_config.json
dotnet build 
```

Before launch bot, edit files in ```./config/```

```./config/database_config.json``` :
```
{
    host: <host>,
    user: <user>,
    password: <password>,
    database: <target database name>
}
```
```./config/bot_config.json``` :
```
{
    token: <BOT token>
}
```

## Maria Database

About database setup: 
```mariadb
CREATE DATABASE `DiscordBot`
```

### Maria tables

About characters data
```mariadb
CREATE TABLE `characters`
(
    `id`         int(11)                                                                                                                           NOT NULL,
    `baseStar`   int(11)                                                                                                                           NOT NULL,
    `position`   enum ('Middle','Back','Front')                                                                                                    NOT NULL,
    `role`       enum ('Attacker','Support','Healer','T.S.')                                                                                       NOT NULL,
    `armorType`  enum ('Light','Heavy','Special','Elastic')                                                                                        NOT NULL,
    `attackType` enum ('Explosive','Penetration','Mystic','Sonic')                                                                                 NOT NULL,
    `weaponType` enum ('AR','FT','GL','HG','MG','MT','RG','RL','SG','SMG','SR')                                                                    NOT NULL,
    `squadType`  enum ('Striker','Special')                                                                                                        NOT NULL,
    `school`     enum ('Abydos','Arius','ETC','Gehenna','Hyakkiyako','Millennium','RedWinter','Shanhaijing','SRT','Sakugawa','Trinity','Valkyrie') NOT NULL,
    PRIMARY KEY (`id`)
);
```

Character Names
```mariadb
CREATE TABLE `character_names`
(
    `id`          int(11)  NOT NULL,
    `name`        tinytext NOT NULL,
    `englishName` varchar(32) DEFAULT '',
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_name` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
);
```

Character Details
```mariadb
CREATE TABLE `character_profiles`
(
    `id`             int(11)  NOT NULL,
    `profile`        longtext NOT NULL,
    `englishProfile` longtext DEFAULT '',
    UNIQUE KEY `id` (`id`),
    KEY `FK_profile` (`id`),
    CONSTRAINT `FK_profile` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
);
```

Character terrain data
```mariadb
CREATE TABLE `character_terrain_damage_dealt`
(
    `id`     int(11) NOT NULL,
    `city`   int(11) DEFAULT 100,
    `desert` int(11) DEFAULT 100,
    `indoor` int(11) DEFAULT 100,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_terrain_damage_dealt` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
);
CREATE TABLE `character_terrain_shield_block_rate`
(
    `id`     int(11) NOT NULL,
    `city`   int(11) DEFAULT 100,
    `desert` int(11) DEFAULT 100,
    `indoor` int(11) DEFAULT 100,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_terrain_shield_block_rate` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
);
```

About user data
```mariadb
CREATE TABLE `users`
(
    `id`       bigint NOT NULL,
    `pyroxene` int(11)          DEFAULT 0,
    `language` enum ('jp','en') DEFAULT 'jp',
    PRIMARY KEY (`id`)
);
```
User lottery data
```mariadb
CREATE TABLE `user_lottery`
(
    `id`        bigint  NOT NULL,
    `lotteryID` int(11) NOT NULL,
    `quantity`  int(11) DEFAULT 0,
    UNIQUE KEY `id` (`id`, `lotteryID`),
    KEY `FK_lottery_id` (`lotteryID`),
    CONSTRAINT `FK_lottery_id` FOREIGN KEY (`lotteryID`) REFERENCES `characters` (`id`),
    CONSTRAINT `FK_lottery_user` FOREIGN KEY (`id`) REFERENCES `users` (`id`)
);
```
User bonus data
```mariadb
CREATE TABLE `user_bonuses`
(
    `id`           bigint NOT NULL,
    `hundred`      int(11) DEFAULT 0,
    `threeHundred` int(11) DEFAULT 0,
    `thousand`     int(11) DEFAULT 0,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_bonus_user` FOREIGN KEY (`id`) REFERENCES `users` (`id`)
);
```
User gacha point data
```mariadb
CREATE TABLE `user_gacha`
(
    `id`    bigint NOT NULL,
    `point` int(11) DEFAULT 0,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_gacha_user` FOREIGN KEY (`id`) REFERENCES `users` (`id`)
);
```

Log user updates when data is modified.
```mariadb
CREATE TABLE `log_user_value_changes`
(
    `id`                int(11)    NOT NULL AUTO_INCREMENT,
    `userID`            bigint(20) NOT NULL,
    `pyroxene`          int(11)         DEFAULT 0,
    `lotteryAmount`     int(11)         DEFAULT 0,
    `gacha`             int(11)         DEFAULT 0,
    `bonusHundred`      int(11)         DEFAULT 0,
    `bonusThreeHundred` int(11)         DEFAULT 0,
    `bonusThousand`     int(11)         DEFAULT 0,
    `time`              timestamp  NULL DEFAULT current_timestamp(),
    PRIMARY KEY (`id`),
    KEY `FK_log_user_value_change` (`userID`),
    CONSTRAINT `FK_log_user_value_change` FOREIGN KEY (`userID`) REFERENCES `users` (`id`)
);
```

## And already setup, that can run it.
```bash
dotnet run
```



