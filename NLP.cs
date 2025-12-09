using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SPLAI_2
{
    internal class NLP
    {
        string pronom = "ist|war|wurde|anbietet|fängt an|kommt an|macht an|meldet an|ruft an|antwortet|zieht an|arbeitet|ärgert|hört auf|macht auf|passt auf|wacht auf|macht aus|ruht aus|sieht aus|steigt aus|zieht aus|badet|bedeutet|beginnt|bekommt|berührt|besichtigt|besucht|bewegt|bezahlt|bittet|bleibt|braucht|bricht|bringt|dankt|denkt|drückt|darf|duscht|lädt ein|steigt ein|empfängt|empfiehlt|endet|entscheidet|entschuldigt|erinnert|erklärt|erlaubt|erzählt|isst|fährt|fällt|fängt|fehlt|feiert|findet|fliegt|folgt|fragt|freut|frühstückt|fühlt|führt|gibt|gefällt|geht|gehört|glaubt|grüßt|guckt|hat|hält|hängt|hasst|hebt|heiratet|heißt|hilft|hofft|holt|hört|informiert|interessiert|kauft|kennt|lernt kennen|kocht|kommt|kann|kostet|küsst|lächelt|lacht|lässt|läuft|lebt|legt|tut leid|lernt|liest|liebt|liegt|lügt|macht|malt|meint|mietet|mag|muss|nimmt|öffnet|packt|passt|passiert|probiert|putzt|raucht|redet|regnet|reist|rennt|repariert|riecht|ruft|sagt|schafft|scheint|schenkt|schickt|schiebt|schießt|schläft|schlägt|schließt|schmeckt|schneit|schneidet|schreibt|schreit|schwimmt|sieht|ist|sendet|setzt|singt|sitzt|soll|spielt|spricht|springt|steckt|steht|stiehlt|stellt|stirbt|stört|streicht|streitet|studiert|sucht|tanzt|teilt|trägt|träumt|trifft|trinkt|tut|verabschiedet|verbietet|verdient|vergisst|vergleicht|verkauft|verliert|vermietet|vermisst|versteckt|versteht|versucht|bereitet vor|stellt vor|wächst|wählt|wartet|wäscht|weckt|tut weh|weint|wird|wirft|wiederholt|weiß|wohnt|will|wünscht|zählt|zeichnet|zeigt|zieht zu|bietest an|fängst an|kommst an|machst an|meldest an|rufst an|antwortest|ziehst an|arbeitest|ärgerst|hörst auf|machst auf|passt auf|wachst auf|machst aus|ruhst aus|siehst aus|steigst aus|ziehst aus|badest|bedeutest|beginnst|bekommst|berührst|besichtigst|besuchst|bewegst|bezahlst|bittest|bleibst|brauchst|brichst|bringst|dankst|denkst|drückst|darfst|duschst|lädst ein|steigst ein|empfängst|empfiehlst|endest|entscheidest|entschuldigst|erinnerst|erklärst|erlaubst|erzählst|isst|fährst|fällst|fängst|fehlst|feierst|findest|fliegst|folgst|fragst|freust|frühstückst|fühlst|führst|gibst|gefällst|gehst|gehört|glaubst|grüßt|guckst|hast|hältst|hängst|hasst|hebst|heiratest|heißt|hilfst|hoffst|holst|hörst|informierst|interessierst|kaufst|kennst|lernst kennen|kochst|kommst|kannst|kostest|küsst|lächelst|lachst|lässt|läufst|lebst|legst|tut leid|lernst|liest|liebst|liegst|lügst|machst|malst|meinst|mietest|magst|musst|nimmst|öffnest|packst|passt|passiert|probierst|putzt|rauchst|redest|regnet|reist|rennst|reparierst|riechst|rufst|sagst|schaffst|scheinst|schenkst|schickst|schiebst|schießt|schläfst|schlägst|schließt|schmeckst|schneit|schneidest|schreibst|schreist|schwimmst|schaust|bist|sendest|setzt|singst|sitzt|sollst|spielst|sprichst|springst|steckst|stehst|stiehlst|stellst|stirbst|störst|streichst|streitest|studierst|suchst|tanzt|teilst|trägst|träumst|triffst|trinkst|tust|verabschiedest|verbietest|verdienst|vergisst|vergleichst|verkaufst|verlierst|vermietest|vermisst|versteckst|verstehst|versuchst|bereitest vor|stellst vor|wächst|wählst|wartest|wäschst|weckst|tust weh|weinst|wirst|wirfst|wiederholst|weißt|wohnst|willst|wünschst|zählst|zeichnest|zeigst|ziehst zu";
        List<ObjectEntity> entities = new List<ObjectEntity>();


        public void ProcessText(string inputText)
        {
            
        }


        public bool Ask(string question)
        {
            return true;
        }

    }
}
