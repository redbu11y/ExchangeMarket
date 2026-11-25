# ExchangeMarket Api

## Krav
Prosjektet krever .NET 10 og EF Core.

For å klone repoet
```bash
git clone https://github.com/redbu11y/ExchangeMarket.git
cd ExchangeMarket
dotnet run --project ExchangeMarket.Api
```

Tesmiljøet for Scalar finnes på `https://localhost:<port>/scalar/v1` og OpenAPI på `https://localhost:<port>/openapi/v1.json`

## Database
Databasen migrerers ved oppstart og fylles med seed dataen. Vi trenger ikke å migrere databasen manuelt. For å resette databasen kan man slette databasefilen `exchangemarket.db`.


## Brukere 
Brukerinnlogging er seedet til følgende.

| Brukernavn     | Passord      | Navn        | Rolle     |
| -------------- | ------------ | ----------- | --------- |
| admin@brukt.no | Admin123!    | Admin       | AdminRole |
| ola@brukt.no   | Bruker123!   | Ola Bruker  | UserRole  |
| kari@brukt.no  | Bruker1234!  | Kari Bruker | UserRole  |
| per@brukt.no   | Bruker12345! | Per Bruker  | UserRole  |

## Testdata
- 4 Kategorier (Elektronikk, Sykler, Møbler, Turutstyr)
- 42 annonser, der 23 er solgt i flere tillstander.
- 8 salg av elektronikk, 7 salg av sykkler, 6 salg av møbler. Disse har tillstrekkelig grunnlag for prisestimater og vurderinger.
- Turutstyr har kun 2 salg og derfor ikke tilstrekkelig data.
- Annonsene har flere statuser, slik som akseptert, avslått, trukket tilbake og på vent.

## Endepunkter

**Auth:**
- `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me`, `PUT /api/auth/me`

**Kategorier**
- `GET /api/categories`, `GET /api/categories/{id}`

**Annonser**
- `GET /api/listings?search=&categoryId=&condition=&minPrice=&maxPrice=&sort=`,  `GET /api/listings/{id}`, `GET /api/listings/my`, `POST /api/listings`

**Tilbud:**
- `GET /api/listings/{id}/offers`, `POST /api/listings/{id}/offers`, `GET /api/offers/my`, `POST /api/offers/{id}/accept`, `POST /api/offers/{id}/reject`, `POST /api/offers/{id}/withdraw`

**Favoritter:**
- `GET /api/favorites`, `POST/DELETE /api/favorites/{listingId}`

**Analyse:**
- `GET /api/analytics/price-suggestion?categoryId=&condition=`, `GET /api/analytics/categories/{id}/stats`, `GET /api/analytics/overview`, `GET /api/analytics/listings/{id}/price-history`

## Struktur

```
ExchangeMarket.Api/
├── Controllers/     Kun HTTP-endepunkter
├── DTOs/          Modeller for forespørsler og svar (entiteter returneres aldri)
├── Models/        Entiteter, enum-typer, roller
├── Repositories/   EF Core query
├── Service/       Forretningsregler og analyse
│   └── Analytics/     Tilstandsfaktorer, statistikk, prisanalyse
├── Exceptions/          Spesifikke unntakstyper og global håndtering av unntak
├── Data/            AppDbContext, DbSeeder, migreringer
└── Program.cs
ExchangeMarket.Tests/  xUnit-tester
```
