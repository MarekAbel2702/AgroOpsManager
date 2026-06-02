# AgroOps Manager

AgroOps Manager to aplikacja webowa napisana w ASP.NET Core MVC, służąca do zarządzania operacjami w gospodarstwie rolnym.

Aplikacja umożliwia zarządzanie polami, maszynami, magazynem, pracami polowymi, kosztami, raportami oraz udostępnia wybrane dane przez REST API dokumentowane w Swaggerze.

Projekt został przygotowany jako aplikacja portfolio, pokazująca praktyczne użycie ASP.NET Core MVC, Entity Framework Core, SQL Server, architektury warstwowej, logiki biznesowej, REST API oraz testów automatycznych.

---

## Opis projektu

AgroOps Manager to system wspierający zarządzanie gospodarstwem rolnym.

Aplikacja pozwala planować i rozliczać prace polowe, kontrolować magazyn, monitorować stan maszyn oraz analizować koszty działalności. Głównym procesem biznesowym jest zakończenie pracy polowej wraz z rozliczeniem zużytych materiałów z magazynu.

Przykładowy scenariusz użycia:

1. Użytkownik dodaje pole uprawne.
2. Użytkownik dodaje maszyny rolnicze.
3. Użytkownik dodaje zasoby magazynowe, np. nawozy, paliwo lub nasiona.
4. Użytkownik planuje pracę polową.
5. Po wykonaniu pracy użytkownik kończy ją i podaje zużyte materiały.
6. System automatycznie zmniejsza stan magazynowy.
7. System oblicza koszt pracy, koszt materiałów i koszt całkowity.
8. Dane trafiają do dashboardu oraz raportów.

---

## Najważniejsze funkcje

Aplikacja zawiera:

- dashboard operacyjny,
- zarządzanie polami,
- zarządzanie maszynami,
- zarządzanie magazynem,
- zarządzanie pracami polowymi,
- proces zakończenia pracy z rozliczeniem magazynu,
- alerty niskich stanów magazynowych,
- alerty serwisowe maszyn,
- raporty kosztów i wartości magazynu,
- REST API,
- dokumentację Swagger/OpenAPI,
- testy jednostkowe,
- testy serwisów biznesowych,
- testy integracyjne API,
- profesjonalny panel administracyjny z sidebarem.

---

## Dashboard

Dashboard pokazuje najważniejsze dane operacyjne gospodarstwa.

Widoczne są między innymi:

- liczba aktywnych pól,
- liczba maszyn,
- liczba zaplanowanych prac,
- liczba zakończonych prac,
- liczba maszyn wymagających serwisu,
- liczba pozycji magazynowych z niskim stanem,
- łączny koszt prac polowych,
- wartość magazynu,
- najbliższe prace polowe,
- ostatnio zakończone prace,
- alerty serwisowe,
- alerty magazynowe.

Dashboard korzysta z rzeczywistych danych z bazy, a nie ze statycznych wartości.

---

## Moduł pól

Moduł pól pozwala zarządzać polami uprawnymi.

Funkcje:

- dodawanie pól,
- edycja pól,
- usuwanie pól,
- podgląd szczegółów pola,
- filtrowanie pól,
- zapisywanie lokalizacji,
- zapisywanie powierzchni w hektarach,
- określanie typu gleby,
- określanie aktualnej uprawy,
- zarządzanie statusem pola,
- podgląd historii prac polowych dla danego pola,
- obliczanie łącznego kosztu prac dla pola,
- obliczanie kosztu na hektar.

Przykładowe dane pola:

- nazwa,
- lokalizacja,
- powierzchnia,
- typ gleby,
- aktualna uprawa,
- status,
- notatki.

---

## Moduł maszyn

Moduł maszyn służy do ewidencji maszyn rolniczych oraz kontroli serwisu.

Funkcje:

- dodawanie maszyn,
- edycja maszyn,
- usuwanie maszyn,
- podgląd szczegółów maszyny,
- filtrowanie maszyn,
- zapisywanie typu maszyny,
- zapisywanie numeru seryjnego,
- zapisywanie roku produkcji,
- kontrola aktualnych motogodzin,
- zapis motogodzin przy ostatnim serwisie,
- ustawianie interwału serwisowego,
- automatyczne wykrywanie maszyn wymagających serwisu,
- podgląd historii prac wykonanych maszyną.

## Moduł magazynu

Moduł magazynu pozwala kontrolować zasoby wykorzystywane w gospodarstwie.

Obsługiwane typy zasobów:

- nawozy,
- paliwo,
- nasiona,
- środki ochrony roślin,
- części zamienne,
- inne zasoby.

Funkcje:

- dodawanie pozycji magazynowych,
- edycja pozycji,
- usuwanie pozycji,
- podgląd szczegółów,
- filtrowanie zasobów,
- kontrola aktualnej ilości,
- ustawianie minimalnego stanu,
- ustawianie ceny jednostkowej,
- zapisywanie jednostki, np. kg, L, szt.,
- zapisywanie dostawcy,
- zapisywanie daty ważności,
- utomatyczne wykrywanie niskich stanów,
- wyliczanie wartości magazynu,
- historia użycia materiału w pracach polowych.

## Moduł prac polowych

Moduł prac polowych jest głównym modułem biznesowym aplikacji.

Funkcje:

- planowanie pracy polowej,
- przypisanie pola,
- przypisanie maszyny,
- wybór typu pracy,
- ustawienie planowanej daty,
- ustawienie statusu,
- zapis kosztu robocizny,
- zapis operatora,
- zapis notatek,
- zakończenie pracy,
- dodanie zużytych materiałów,
- automatyczne zmniejszenie stanu magazynowego,
- zapis kosztu materiałów w momencie użycia,
- wyliczenie kosztu robocizny,
- wyliczenie kosztu materiałów,
- wyliczenie kosztu całkowitego,
- wyliczenie kosztu na hektar.

Statusy prac:

- Planned,
- InProgress,
- Completed,
- Cancelled.

## Raporty

Aplikacja zawiera moduł raportów i analityki.

Raporty pokazują:

- łączny koszt prac polowych,
- koszt robocizny,
- koszt zużytych materiałów,
- wartość magazynu,
- koszty według typu pracy,
- koszty według pola,
- koszt na hektar,
- wartość magazynu według kategorii,
- liczbę prac według statusu,
- najdroższe prace polowe.

Raporty są generowane na podstawie rzeczywistych danych zapisanych w bazie.

## REST API i Swagger

Aplikacja udostępnia wybrane dane przez REST API.

Swagger jest dostępny w środowisku developerskim pod adresem:

/swagger

Przykładowe endpointy:

GET /api/health-check
GET /api/dashboard/summary
GET /api/fields
GET /api/fields/{id}
GET /api/machines/service-alerts
GET /api/inventory/low-stock
GET /api/field-works/upcoming

Dzięki Swaggerowi można testować endpointy bezpośrednio z przeglądarki.

## Technologie

W projekcie użyto:

- ASP.NET Core MVC,
- Entity Framework Core,
- SQL Server,
- EF Core InMemory dla testów,
- Razor Views,
- Bootstrap,
- Swagger,
- xUnit,
- FluentAssertions,
- Microsoft.AspNetCore.Mvc.Testing.

## Jak uruchomić projekt lokalnie

Poniżej znajduje się instrukcja uruchomienia projektu na lokalnym komputerze.

---

### 1. Wymagania

Do uruchomienia projektu potrzebne są:

- .NET SDK 8 lub nowszy,
- Visual Studio 2022 albo Visual Studio Code,
- SQL Server LocalDB albo lokalny SQL Server,
- Entity Framework Core CLI.

Sprawdzenie wersji .NET:

```powershell
dotnet --version

Jeżeli narzędzie dotnet ef nie jest zainstalowane, można je dodać poleceniem:

```dotnet tool install --global dotnet-ef

Jeżeli jest już zainstalowane, można je zaktualizować:

```dotnet tool update --global dotnet-ef

## 2. Pobranie projektu

Po sklonowaniu repozytorium przejdź do głównego folderu projektu, czyli folderu zawierającego plik:

AgroOpsManager.sln

Przykład:

cd AgroOpsManager

## 3. Przywrócenie paczek NuGet

W głównym folderze rozwiązania uruchom:

dotnet restore

## 4. Konfiguracja połączenia z bazą danych

Connection string znajduje się w pliku:

AgroOpsManager/appsettings.json

Przykładowa konfiguracja dla SQL Server LocalDB:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgroOpsManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}

Alternatywnie można użyć lokalnego SQL Servera:

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Database=AgroOpsManagerDb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"
  }
}

## 5. Utworzenie migracji

Jeżeli migracja nie istnieje jeszcze w projekcie, utwórz ją poleceniem:

dotnet ef migrations add InitialCreate --project ".\AgroOpsManager.Infrastructure\AgroOpsManager.Infrastructure.csproj" --startup-project ".\AgroOpsManager\AgroOpsManager.Web.csproj" --output-dir "Data\Migrations"

Jeżeli migracja już istnieje, ten krok można pominąć.

## 6. Utworzenie lub aktualizacja bazy danych

Uruchom:

dotnet ef database update --project ".\AgroOpsManager.Infrastructure\AgroOpsManager.Infrastructure.csproj" --startup-project ".\AgroOpsManager\AgroOpsManager.Web.csproj"

Polecenie utworzy bazę danych oraz tabele na podstawie migracji.

Po pierwszym uruchomieniu aplikacja automatycznie doda przykładowe dane startowe, między innymi:

pola,
maszyny,
pozycje magazynowe,
przykładową pracę polową,
przykładowe zużycie materiału.

## 7. Uruchomienie aplikacji

W głównym folderze rozwiązania uruchom:

dotnet run --project ".\AgroOpsManager\AgroOpsManager.Web.csproj"

Po uruchomieniu w konsoli pojawi się adres aplikacji, np.:

https://localhost:7000

Otwórz ten adres w przeglądarce.

## 8. Swagger API

Dokumentacja API jest dostępna pod adresem:

/swagger

Przykładowe endpointy:

GET /api/health-check
GET /api/dashboard/summary
GET /api/fields
GET /api/fields/{id}
GET /api/machines/service-alerts
GET /api/inventory/low-stock
GET /api/field-works/upcoming
## 9. Uruchomienie testów

Aby uruchomić wszystkie testy, użyj:

dotnet test

Projekt zawiera:

testy jednostkowe encji domenowych,
testy serwisu biznesowego,
testy integracyjne API.

Testy integracyjne używają bazy InMemory, więc nie wymagają lokalnej bazy SQL Server.
