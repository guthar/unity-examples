# Story des Spiels

## Figur

Die Spielfigur ist der Raiffeisen Club Hecht. (-> Asset kaufen)
Man sieht die Figur eigentlich nicht, blickt man jedoch zurück, sieht man die Schwanzflosse.

## Landschaft/Umgebung

Man schwimmt unter Wasser in Linz rund um die Landstraße zwischen Volksgarten und Hauptplatz bzw. teilweise auch durch Hinterhöfe und Plätze. (-> Andi exportiert)
Generell wird am Himmel immer die Sonne angezeigt, ggf. kann die Landschaft über Wasser der aktuellen Jahreszeit angepasst werden, wobei die Sunne immer sichtbar sein muss. (-> Sonne existiert)
Auf der Landstraße wird die neue, moderne Bankfiliale dargestellt, zu der man das gesammelte Geld bringen muss. (-> offen)

## Spielverlauf

### Spielstart

Der Start des Spiels erfolgt immer von der Position aus, bei der das vorige Spiel beendet wurde.

### Verlauf

Man schwimmt unter Wasser in der Stadt herum auf der Suche nach Geld. (-> Schwimmen unter Wasser erledigt)
Dieses Geld schwebt auf unterschiedlichen Niveaus, die teilweise unter Wasser, teilweise aber auch in der Luft sein können. (-> unterschiedliche Höhe noch offen)
In der Luft schwebendes Geld kann man durch leuchtende Reflexionen unter Wasser erkennen. (-> variable Punkteberechnung erledigt)
Unter Wasser kann Geld durch einfaches Durchschwimmen eingesammelt werden, über Wasser muss man versuchen dieses durch einen Sprung aus dem Wasser zu erwischen. (-> Sprung aus Wasser möglich, Collision mit Geldobjekten erledigt)
Das Geld erscheint in bestimmten Intervallen an zufälligen Orten am Spielfeld und bleibt dort bestehen. (-> erledigt)

### Spielende

Ist die definierte Spielzeit abgelaufen, wird das Spiel beendet. (-> offen)

## Punkte

Punkte können durch folgende Aktionen erreicht werden:
* Einsammeln von Geld (unter bzw. über Wasser) (-> erledigt)
* Bringen des eingesammelten Geldes zur Bank (Zinsen) (-> offen)
* Bringen des eingesammelten Geldes zur Bank am Weltspartag (zusätzliche Zinsen) (-> offen)

Die aktuelle Punkteanzahl wird an mehreren Stellen in der Stadt auf Displays angezeigt (z.B. Passage). (-> offen)

## Besonderheiten

### Weltspartag

Die Sonne am Himmel strahlt immer, zu manchen Zeitpunkten wird in der Sonne ein Raiffeisen-Giebelkreuz angezeigt, welches den Weltspartag anzeigt. (-> erledigt)
Wird das gesammelte Geld zu diesen Zeiten in der Bankstelle abgeliefert, erhält man nicht nur die Zinsen, sondern zusätzlich ein Power-up. (-> offen)

#### Power-ups

* Schneller schwimmen
* Höher springen
* Wasserspiegel steigen lassen
* ...

### Wasserspiegel

Der Wasserspiegel hat einen definierten Tiefststand, kann jedoch vom Spieler beeinflusst werden.
Schwimmt man bis zur Donau, kann man dort einen Damm aktivieren, der das Wasser aufstaut und Linz mehr und mehr bis zu einem definierten Höchststand flutet.
Dadurch sind mehr Geldstücke unter Wasser und können kontrollierter eingesammelt werden.
Der Damm hält eine definierte Anzahl von Spielen (z.B. 3) und wird danach wieder abgebaut.
Da das Einsammeln von Geld bei höherem Wasserspiegel leichter ist als bei niedrigem, soll es ein Anreiz für den Spieler sein, vor Ende seines Spiels den Damm wieder abzubauen, denn dieser Zustand bleibt beim nächsten Spiel erhalten. So kann er die Ausgangssituation des Folgespielers beeinflussen.

(-> offen)

### Gegner

#### Fischer

Auf der Wasseroberfläche fährt ein Fischerboot, das immer wieder Fangnetze ins Wasser wirft.
Wird man von den Fischern erwischt, ist das bisher gesammelte Geld weg und man wird einige Sekunden im Netz festgehalten.
Danach kann man dem Netz entkommen und das Spiel fortsetzen. (-> offen)

Fischerboot + Netz Asset (-> erledigt)

#### Zeit

Die Spieldauer wird mit 5 Minuten begrenzt. Nach dieser Zeit wird die Punkteanzahl in der Hiscore-Liste festgeschrieben und das nächste Spiel beginnt wieder bei 0.
Gesammeltes, jedoch nicht abgeliefertes Geld wird nach Spielende sofort wieder am Spielfeld verteilt.

## Sonstiges

### Menü

(-> offen)

### Schwierigkeitsgrade

(-> kein Thema für den Hackathon)

### Sonstiges für Pitch

* andere Fische
* evtl. Gegner in anderer Bankstelle über Netzwerk einbinden
