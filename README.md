O projektu:
Serverska aplikacija koja prima HTTP zahteve, pretrazuje fajlove i broji palindrome.
Sistem podrzava veci broj zahteva koriscenjem ThreadPool-a.
Strategija za kes memoriju: ogranicenje velicine.

Sistem je podeljen na sledece komponente:
-HTTPServer-prima zahteve i smesta ih u red
-RequestQueue-deljena struktura podataka
-FileExplorer-obradjuje zahteve i broji palindrome
-LRUCache-kes za cuvanje prethodno izracunatih rezultata
-Logger-thread-safe logovanje

-MEHANIZMI SINHRONIZACIJE:
-lock/Monitor
Koriscen u RequestQueue za sinhronizaciju pristupa redu zahteva.
-Monitor.Wait oslobadja lock i blokira nit dok ne stigne signal.
-Monitor.PulseAll-budi sve niti koje cekaju.

-Monitor u FileExplorer
inProgress directory cuva fajlove koji su trenutno u obradi.
Kada vise niti zahteva isti fajl, samo jedna nit radi obradu dok ostale cekaju koristeci Monitor.Wait.
Nakon toga Monitor.PulseAll budi sve cekajuce niti koje zatim uzimaju rezultat iz kesa.

-SemaphoreSlim
Ogranicava broj paralelnih obrada fajlova na 3.
Sprecava preopterecenje servera pri velikom broju zahteva.

-LRUCache:
Thread-Safe implementacija koriscenjem lock mehanizma.
Kapacitet kesa je 5(u ovom konkretnom zadatku da bi lako videli kako funkcionise ogranicenje velicine)
Kada je kes pun, izbacuje se najredje korisceni element.

KRITICNE SEKCIJE:
Pristup redu zahteva-je zasticen kombinacijom lock i Monitor mehanizma.
Razlog je visenitni pristup Queue strukturi gde producer i consumer niti istovremeno
dodaju i uzimaju zahteve.

Pristup inProgress recniku je zasticen lock i Monitor mehanizmima.
Provera da li je fajl u obradi i upis moraju biti nedeljivi kako bi se sprecilo da dve niti preuzmu obradu istog fajla.

Pristup LRU kes strukturi je zasticen lock mehanizmom.
Visenitno citanje i pisanje u kes moraju biti sinhronizovani kako bi se ocuvala konzistentnost podataka.

Logovanje je zasticeno lock mehanizmom kako bi se sprecilo ispreplitanje log poruka razlicitih niti, sto bi log ucinilo necitljivim.

Cache Stampede
Problem nastaje kada vise niti istovremeno zahteva isti resurs koji nije u kesu, sto bi uzrokovalo visestruku obradu istog fajla.

Resenje u ovom projektu:
Prva nit koja detektuje cache miss upisuje fajl u inProgress i preuzima obradu.
Sve ostale niti detektuju da je obrada u toku i blokiraju se koristeći Monitor.Wait.
Nakon završetka obrade, rezultat se upisuje u keš i poziva se Monitor.PulseAll.
Sve čekajuće niti se bude i uzimaju rezultat iz keša

PONASANJE PRI OPTERECENJU

Mali broj zahteva(do 3)
Svaki zahtev se obradjuje paralelno bez cekanja na semafor.
Sistem radi optimalno.

Srednji broj zahteva(od 4 do 10)
Semafor ograničava paralelnu obradu na 3 niti.
Ostali zahtevi čekaju u redu ili na oslobađanje semafora.
Cache stampede zaštita sprečava višestruku obradu istih fajlova.

Veliki broj zahteva (10+)
RequestQueue ograničava red na 5 zahteva.
Ako je red pun, producer nit čeka dok se ne oslobodi mesto.
Sistem ostaje stabilan zahvaljujući kombinaciji semafora i reda zahteva.