# RESTpectfulScraper

This simple scraper harvests Yu-Gi-Oh card data from a .csv in the `/input` directory and turns them into a game-formatted .csv placed in the `/output` directory. 

### Sourcing data

The .csv feeding this scraper is generated from .cdb files used to power EDO Pro, the popular Yu-Gi-Oh online simulator. You can actually extract most of the card list from EDO Pro's program files, but that appears to be an incomplete archive that cuts off sometime before the end of 2022. For cards released since then, I had to UNION the EDO Pro "base" .cdb file with .cdb files sourced from [ProjectIgnis/DeltaPuppetOfStrings](https://github.com/ProjectIgnis/DeltaPuppetOfStrings). Specifically, I used `cards.delta.cdb` because it appeared to contain the collection with all TCG data confirmed by printing instead of pre-release fan interpretation. This repo, [BabelCDB](https://github.com/ProjectIgnis/BabelCDB) may be better suited for what I want to do. That's something worth investigating.

To manually generate .csv files based on .cdb files, use [DB Browser for SQLite](https://sqlitebrowser.org/). Open the main .cdb file with this tool, then open `cards.delta.cdb` within the db context by clicking the "attach database" button and following the instructions. The only values we need for our .csv are  `texts.id` and `texts.name`. 

---

### Future goals

1. Automate the data collection process	

... When cards get added to `cards.delta.cdb`, the EDO Pro maintainers make a commit. If we can trigger an update to our .csv whenever that file changes, we're in business.
2. Record all intermediate API responses

 ... As a gut check, it would be great if we could cache the json responses from the [ygoprodeck API](https://ygoprodeck.com/api-guide/) in the `/output` directory. There's juicy data in those responses that could be helpful later on.

