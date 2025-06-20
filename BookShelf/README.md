BookShelf sistem
🏠 Glavna stranica (MainView)
Pretraga i filtriranje
•	Search polje:
o	Pretražuje knjige po naslovu ili imenu autora
o	Pretraga se izvršava automatski tokom kucanja
•	Category dropdown:
o	Filtrira knjige po kategorijama
o	“All” opcija prikazuje sve
•	Author dropdown:
o	Filtrira po autorima
o	“All” prikazuje sve autore
•	Clear Filters dugme: Uklanja sve primijenjene filtere
Lista knjiga
•	DataGrid: Prikazuje listu knjiga sa osnovnim informacijama
o	Naslov, autor, kategorija, izdavač, godina, cijena, količina
•	View Details dugme:
o	Otvara detaljan prikaz selektovane knjige
o	Aktivno samo ako je knjiga selektovana
•	Add to Cart dugme:
o	Dodaje knjigu u korpu
o	Aktivno samo ako je knjiga selektovana i dostupna
Korisnički meni
•	View Cart: Otvara prozor korpe
•	Logout: Odjava korisnika
•	Tema:
o	Light – Svijetla
o	Dark – Tamna
o	Crazy – Kreativna, šarena
o	Izbor teme se automatski pamti u bazi
________________________________________
👑 Administratorski meni (vidljiv samo adminima)
•	Manage Books: Upravljanje knjigama
•	Manage Users: Upravljanje korisnicima
•	Manage Orders: Upravljanje narudžbama
________________________________________
📖 Detalji knjige (BookDetailsView)
Prikaz informacija
•	Naslov, autor, izdavač, kategorija, godina
•	Opis knjige
•	Cijena i količina
•	Prosječna ocjena (prema recenzijama)
•	Broj recenzija
Recenzije
•	Lista recenzija: Ime korisnika, ocjena (1–5 zvjezdica), komentar, datum
•	Forma za novu recenziju:
o	Ocjena: Klikom na zvjezdice
o	Komentar: Tekstualni
o	Submit: Aktivno samo ako je korisnik prijavljen i komentar unesen
Akcije
•	Add to Cart: Dodaje u korpu
•	Close: Zatvara prozor
________________________________________
🛒 Korpa (ShoppingCartView)
Pregled sadržaja
•	Naslov, cijena, količina, ukupno
•	Ukupna cijena se automatski ažurira

Upravljanje stavkama
o	+ i – dugmad: Povećavaju / smanjuju količinu
•	Remove: Uklanja stavku
Završetak kupovine
•	Checkout:
o	Kreira narudžbu
o	Aktivno ako korpa nije prazna
o	Briše korpu i prikazuje potvrdu
________________________________________
📚 Administracija knjiga (AdminBooksView)
Pregled i selekcija
•	Lista knjiga (DataGrid)
•	Klik kopira podatke u formu
Forma za unos/editovanje
•	Naslov, opis, godina, cijena, količina, autor, izdavač, kategorija
•	Putanja do slike
Akcije
•	Add New
•	Save (aktivno ako su obavezna polja popunjena)
•	Delete (sa potvrdom)
________________________________________
👥 Administracija korisnika (AdminUsersView)
Pregled korisnika
•	Lista svih korisnika (ime, prezime, email, uloga)
Forma za unos/editovanje
•	Ime, prezime, email, uloga
•	Lozinka (obavezna za nove, opcionalna za postojeće)


Akcije
•	Add New
•	Save (provjera validnosti)
•	Delete (sa potvrdom, ne može obrisati trenutno prijavljenog)
________________________________________
📋 Administracija narudžbi (AdminOrdersView)
Pregled
•	Lista narudžbi: ID, datum, status, ukupna cijena
•	Detalji: Stavke narudžbe, kontakt korisnika
Upravljanje statusom
•	Promjena statusa: Processing, Shipped, Completed, Cancelled
•	Aktivno samo pri selekciji
•	Traži potvrdu
•	Ažurira u bazi i prikazu
________________________________________
🎨 Tema sistema
Sistem podržava tri teme:
•	Light Theme: Svijetla pozadina, tamni tekst
•	Dark Theme: Tamna pozadina, svijetli tekst
•	Crazy Theme: Šarena tema s gradijentima
➡️ Izbor teme se automatski pamti u bazi i učitava pri sljedećoj prijavi korisnika.
________________________________________
🔧 Tehnički detalji
•	Arhitektura: MVVM pattern
•	WPF: Za korisnički interfejs
•	Data Binding: Automatska sinhronizacija
•	Commands & ObservableCollection
•	Validacija: U realnom vremenu
•	Bezbijednost:
o	Pristup po ulozi
o	Potvrde za osjetljive akcije
o	Validacija unosa
o	Upravljanje sesijom korisnika
________________________________________
🚀 Pokretanje sistema
1.	Pokrenite aplikaciju
2.	Prijavite se ili registrujte
3.	Koristite meni za navigaciju
4.	Odjavite se kada završite
________________________________________