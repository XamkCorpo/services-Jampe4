# Vaihe 3: Service-kerros, Repository, Result Pattern ja API-dokumentaatio — Teoriakysymykset

Vastaa alla oleviin kysymyksiin omin sanoin. Kirjoita vastauksesi kysymysten alle.

> **Vinkki:** Jos jokin kysymys tuntuu vaikealta, palaa lukemaan teoriamateriaalit:
> - [Service-kerros ja DI](https://github.com/xamk-mire/Xamk-wiki/blob/main/C%23/fin/04-Advanced/WebAPI/Services-and-DI.md)
> - [Repository Pattern](https://github.com/xamk-mire/Xamk-wiki/blob/main/C%23/fin/04-Advanced/Patterns/Repository-Pattern.md)
> - [Result Pattern](https://github.com/xamk-mire/Xamk-wiki/blob/main/C%23/fin/04-Advanced/Patterns/Result-Pattern.md)

---

## Osa 1: Service-kerros

### Kysymys 1: Fat Controller -ongelma

Miksi on ongelma jos controller sisältää kaiken logiikan (tietokantakyselyt, muunnokset, validoinnin)? Anna vähintään kaksi konkreettista haittaa.

**Vastaus:**
Jos controller sisältää kaiken logiikan, siitä tulee helposti liian suuri ja vaikeasti ylläpidettävä.
Ensimmäinen haitta on se, että sama logiikka alkaa helposti toistua useissa controllereissa tai metodeissa.
Toinen haitta on testauksen vaikeutuminen, koska logiikkaa ei ole erotettu omiin kerroksiinsa.

---

### Kysymys 2: Vastuunjako

Miten vastuut jakautuvat controller:n, service:n ja repository:n välillä tässä harjoituksessa? Kirjoita lyhyt kuvaus kunkin kerroksen tehtävästä.

**Controller vastaa:**
Mitä HTTP-pyynnöllä tehdään, minkä endpointin kautta, ja mitä vastataan asiakkaalle.

**Service vastaa:**
Mitä sovellus tekee, miten tekee, ja miten käsitellään erilaisia tilanteita (esim. virheet).

**Repository vastaa:**
Datan hakemisesta, tallentamisesta ja poistamisesta tietokannasta.

---

### Kysymys 3: DTO-muunnokset servicessä

Miksi DTO ↔ Entity -muunnokset kuuluvat serviceen eikä controlleriin? Mitä hyötyä siitä on, että controller ei tunne `Product`-entiteettiä lainkaan?

**Vastaus:**
Koska DTO ↔ Entity -muunnokset kuuluvat serviceen, controller pysyy puhtaana ja keskittyy vain HTTP-pyyntöjen käsittelyyn. 
Tämä tekee controllerista yksinkertaisemman ja helpommin testattavan. Lisäksi, jos controller ei tunne `Product`-entiteettiä lainkaan, 
se tarkoittaa että controller on täysin riippumaton tietokantamallista, mikä parantaa koodin joustavuutta ja ylläpidettävyyttä.

---

## Osa 2: Interface ja Dependency Injection

### Kysymys 4: Interface vs. konkreettinen luokka

Miksi controller injektoi `IProductService`-interfacen eikä suoraan `ProductService`-luokkaa? Mitä hyötyä tästä on?

**Vastaus:**
Koska interface tarjoaa abstraktion, joka mahdollistaa toteutukset ilman että controller tarvitsee tietää niistä.
Koodi pysyy joustavana ja testattavana, koska toteutuksia voidaan vaihtaa helposti muuttamatta controlleria.
Suoraan `ProductService`-luokkaa injektoimalla controller olisi suoraan riippuvainen tietystä toteutuksesta, mikä vaikeuttaisi testauksessa ja ylläpidossa.

---

### Kysymys 5: DI-elinkaaret

Selitä ero näiden kolmen elinkaaren välillä ja anna esimerkki milloin kutakin käytetään:

- **AddScoped:**
AddScoped: instanssi kestää yhden HTTP-pyynnön ajan. Esimerkiksi palvelut, jotka käyttävät DbContextia.

- **AddSingleton:**
AddSingleton: sama instanssi kestää koko sovelluksen elinkaaren ajan.

- **AddTransient:**
AddTransient: uusi instanssi luodaan aina, kun palvelua pyydetään.

Miksi `AddScoped` on oikea valinta `ProductService`:lle?
Koska ProductService käyttää DbContextia, AddScoped on sille oikea valinta.
Saman HTTP-pyynnön aikana käytettävät palvelut käyttävät samaa turvallista contextia.

---

### Kysymys 6: DI-kontti

Selitä omin sanoin mitä DI-kontti tekee kun HTTP-pyyntö saapuu ja `ProductsController` tarvitsee `IProductService`:ä. Mitä tapahtuu vaihe vaiheelta?

**Vastaus:**
Kun HTTP-pyyntö saapuu ja ProductsController tarvitsee IProductService:ä, ASP.NET Core tarkistaa DI-kontista, mikä toteutus IProductService-rajapinnalle on rekisteröity.
Jos ProductService on rekisteröity, DI-kontti luo siitä instanssin.
Jos ProductService tarvitsee konstruktorissaan muita riippuvuuksia, kuten repositoryn tai loggerin, DI-kontti luo tai hakee myös ne.
Lopuksi DI-kontti antaa valmiin ProductService-instanssin controllerille.

---

### Kysymys 7: Rekisteröinnin unohtaminen

Mitä tapahtuu jos unohdat rekisteröidä `IProductService`:n `Program.cs`:ssä? Milloin virhe ilmenee ja miltä se näyttää?

**Vastaus:**
Jos IProductService:ä ei rekisteröidä Program.cs:ssä, ProductsControlleria ei pystytä luomaan, koska IProductService:n toteutusta ei tiedetä.
Virhe ilmenee silloin, kun controlleria yritetään aktivoida pyynnön käsittelyn aikana.
Tyypillinen virhe on System.InvalidOperationException, jossa lukee esimerkiksi:
Unable to resolve service for type ... while attempting to activate ...

---

## Osa 3: Repository-kerros

### Kysymys 8: Miksi repository?

`ProductService` käytti aluksi `AppDbContext`:ia suoraan. Miksi se refaktoroitiin käyttämään `IProductRepository`:a? Anna vähintään kaksi syytä.

**Vastaus:**
Koska tällöin liiketoimintalogiikka saadaan erotettua tietokantalogiikasta.
Repository voidaan mockata yksikkötesteissä ilman oikeaa tietokantaa.
Lisäksi ProductService ei ole enää suoraan riippuvainen EF Coresta tai DbContextista.

---

### Kysymys 9: Service vs. Repository

Mikä on `IProductService`:n ja `IProductRepository`:n välinen ero? Mitä tietotyyppejä kumpikin käsittelee (DTO vai Entity)?

**IProductService:**
käsittelee sovelluslogiikkaa ja käyttää yleensä DTO-olioita, kuten CreateProductRequest ja ProductResponse.

**IProductRepository:**
käsittelee datan hakua ja tallennusta, ja käyttää yleensä entiteettejä, kuten Product.

---

### Kysymys 10: Controllerin muuttumattomuus

Kun Vaihe 7:ssä lisättiin repository-kerros, `ProductsController` ei muuttunut lainkaan. Miksi? Mitä tämä kertoo rajapintojen (interface) hyödystä?

**Vastaus:**
ProductsController ei muuttunut, koska se käytti jo valmiiksi IProductService-rajapintaa eikä ollut sidottu siihen, miten service toteuttaa työnsä sisäisesti.
Kun service muutettiin käyttämään repositorya, muutos jäi service-kerrokseen eikä controlleria tarvinnut muuttaa.
Tämä osoittaa, että rajapinnat vähentävät kytkentää eri kerrosten välillä.

---

## Osa 4: Exception-käsittely ja lokitus

### Kysymys 11: ILogger

Mikä on `ILogger` ja miksi sitä tarvitaan? Mistä lokit näkee kehitysympäristössä?

**Vastaus:**
ILogger on .NETin lokitusrajapinta ja sen avulla sovellus voi kirjoittaa tietoa esim. virheistä, varoituksista sekä normaalista toiminnasta.
Tarvitaan, koska ongelmia voidaan myöhemmin tutkia ilman, että käyttäjälle näytetään teknisiä yksityiskohtia.
Kehitysympäristössä lokit näkyvät konsolissa tai terminaalissa sekä Visual Studiossa Output-ikkunassa.

---

### Kysymys 12: Odotetut vs. odottamattomat virheet

Selitä ero "odotetun" ja "odottamattoman" virheen välillä. Anna esimerkki kummastakin ja kerro miten ne käsitellään eri tavalla servicessä.

**Odotettu virhe (esimerkki + käsittely):**
Esimerkiksi tuotetta ei löydy käyttäjän antamalla id:llä tai data on muuten virheellistä.
Ne kuuluvat normaaliin sovelluksen toimintaan, joten ne kannattaa käsitellä ja palauttaa servicestä Result.Failure(...)-muodossa

**Odottamaton virhe (esimerkki + käsittely):**
Esimerkiksi tietokantayhteys katkeaa tai tapahtuu jokin muu käyttäjästä riippumaton virhe.
Tällaiset virheet eivät kuulu normaaliin sovelluksen toimintaan, joten ne kannattaa lokittaa ILogger:lla ja heittää edelleen exceptionina.

---

## Osa 5: Result Pattern

### Kysymys 13: Miksi null ja bool eivät riitä?

Alla on kaksi esimerkkiä. Selitä miksi ensimmäinen tapa on ongelmallinen ja miten toinen ratkaisee ongelman:

```csharp
// Tapa 1: null
ProductResponse? product = await _service.GetByIdAsync(id);
if (product == null)
    return NotFound();

// Tapa 2: Result
Result<ProductResponse> result = await _service.GetByIdAsync(id);
if (result.IsFailure)
    return NotFound(new { error = result.Error });
```

**Vastaus:**
Eka tapa on ongelmallinen, koska null kertoo vain sen, että jokin meni pieleen, mutta ei kerro mikä.
Ongelma voi olla esimerkiksi se, ettei tuotetta löytynyt, tai jokin muu virhe.
Toinen tapa käyttää Result-oliota, joka sisältää tiedon sekä onnistumisesta että virheviestistä.
Siksi virhetilanteen käsittely on selkeämpää.

---

### Kysymys 14: Result.Success vs. Result.Failure

Miten `Result Pattern` muutti virheiden käsittelyä servicessä? Vertaa Vaihe 8:n `throw;`-tapaa Vaihe 9:n `Result.Failure`-tapaan: mitä eroa niillä on asiakkaan (API:n kutsuja) näkökulmasta?

**Vastaus:**
throw heittää virheen exception-mekanismin kautta, jolloin API:n kutsujalle voi näkyä yleisempi virhe eikä käsittely ole yhtä selkeää.
Result.Failure taas näyttää odotetut virheet hallitulla tavalla: service palauttaa selkeän virheviestin, ja controller voi mapata sen sopivaksi HTTP-vastaukseksi, esimerkiksi 404 NotFound tai 400 BadRequest.
Asiakkaan näkökulmasta virhetilanne on tällöin selkeämpi ja ennustettavampi.

---

## Osa 6: API-dokumentaatio

### Kysymys 15: IActionResult vs. ActionResult\<T\>

Miksi `ActionResult<ProductResponse>` on parempi kuin `IActionResult`? Anna vähintään kaksi syytä.

**Vastaus:**
ActionResult<ProductResponse> on parempi kuin IActionResult, koska se kertoo suoraan onnistuneen vastauksen tyypin.
Se tekee koodista luettavamman ja API-dokumentaatiosta selkeämmän.
Lisäksi samasta metodista voidaan palauttaa sekä ProductResponse että esimerkiksi NotFound() kätevästi.

---

### Kysymys 16: ProducesResponseType

Mitä `[ProducesResponseType]`-attribuutti tekee? Miten se näkyy Swagger UI:ssa?

**Vastaus:**
kertoo, mitä HTTP-statuksia ja vastaustyyppejä action voi palauttaa.
Ei muuta actionin toimintaa, vaan lisää metadataa dokumentaatiota varten. 
Swagger UI:ssa näkyy endpointin alla on listattuna esim. 200m 404 tai 400 vastaukset ja niihin liittyvät tyypit tai skeemat.

---

### Kysymys 18: Refaktorointi

Sovelluksen toiminnallisuus pysyi täysin samana koko harjoituksen ajan — samat endpointit, samat vastaukset. Mitä refaktorointi tarkoittaa ja miksi se kannattaa, vaikka käyttäjä ei huomaa eroa?

**Vastaus:**
Refaktorointi on koodin rakenteen parantamista, ilman että ohjelman ulkoinen toiminnallisuus muuttuu.
harjoituksessa endpointit ja vastaukset pysyivät samoina, mutta koodin rakenne parantui.
vastuut eriytettiin, testattavuus parani ja kytkentä vähentyi kerrosten välillä.
Refaktorointi kannattaa, koska se parantaa koodia ja sitä on näin helpompi ylläpitää, testata ja laajentaa myöhemmin.


---
