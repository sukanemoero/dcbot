# The discord lottery bot using dotnet  

## Image source

https://github.com/SchaleDB/SchaleDB

## Character source

https://github.com/torikushiii/BlueArchiveAPI/

## Mysql Database

```mariadb
CREATE DATABASE `DiscordBot`
```

## Mysql tables

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
)
```
```mariadb
CREATE TABLE `character_names`
(
    `id`          int(11)  NOT NULL,
    `name`        tinytext NOT NULL,
    `englishName` varchar(32) DEFAULT '',
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_name` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
)

```
```mariadb
CREATE TABLE `character_profiles`
(
    `id`             int(11)  NOT NULL,
    `profile`        longtext NOT NULL,
    `englishProfile` longtext DEFAULT '',
    UNIQUE KEY `id` (`id`),
    KEY `FK_profile` (`id`),
    CONSTRAINT `FK_profile` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
)
```
```mariadb
CREATE TABLE `character_terrain_damage_dealt`
(
    `id`     int(11) NOT NULL,
    `city`   int(11) DEFAULT 100,
    `desert` int(11) DEFAULT 100,
    `indoor` int(11) DEFAULT 100,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_terrain_damage_dealt` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
)
```

```mariadb
CREATE TABLE `character_terrain_shield_block_rate`
(
    `id`     int(11) NOT NULL,
    `city`   int(11) DEFAULT 100,
    `desert` int(11) DEFAULT 100,
    `indoor` int(11) DEFAULT 100,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_terrain_shield_block_rate` FOREIGN KEY (`id`) REFERENCES `characters` (`id`)
)
```

```mariadb
CREATE TABLE `users`
(
    `id`       bigint NOT NULL,
    `pyroxene` int(11)          DEFAULT 0,
    `language` enum ('jp','en') DEFAULT 'jp',
    PRIMARY KEY (`id`)
)
```

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
)
```

```mariadb
CREATE TABLE `user_bonuses`
(
    `id`           bigint NOT NULL,
    `hundred`      int(11) DEFAULT 0,
    `threeHundred` int(11) DEFAULT 0,
    `thousand`     int(11) DEFAULT 0,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_bonus_user` FOREIGN KEY (`id`) REFERENCES `users` (`id`)
)
```

```mariadb
CREATE TABLE `user_gacha`
(
    `id`    bigint NOT NULL,
    `point` int(11) DEFAULT 0,
    UNIQUE KEY `id` (`id`),
    CONSTRAINT `FK_gacha_user` FOREIGN KEY (`id`) REFERENCES `users` (`id`)
)
```


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
)
```

```mariadb
CREATE TABLE `lottery_message_characters`
(
    `id`          int(11) NOT NULL,
    `characterID` int(11) NOT NULL,
    KEY `FK_message_id` (`id`),
    KEY `FK_message_character_id` (`characterID`),
    CONSTRAINT `FK_message_character_id` FOREIGN KEY (`characterID`) REFERENCES `characters` (`id`),
    CONSTRAINT `FK_message_id` FOREIGN KEY (`id`) REFERENCES `lottery_messages` (`id`)
)
```





