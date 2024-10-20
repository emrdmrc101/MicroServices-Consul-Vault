Consul - Service Discovery (Servis Keşfi) ve Vault - Secrets Management örneği

Consul : Servislerimizin sağlık durumu ve keşfi için kullandık. Tüm servislerin adres bilgilerinin merkezi biryerden yürütülmesi. Her servis çalıştığında kendini consule belirli bir name, url ve id ile kaydeder.
Consul bu bilgiler ile servisin sağlık durumunu gözlemler ve bilgilendirir. Servisin diğer servislerin bilgilerine ise yine consul aracılığı ile erişir. O anki url bilgisi vb..

Vault: Hassas bilgilerin güvenli bir şekilde saklanmasını sağlar. database, redis gibi connection bilgileri merkezi bir şekilde yönetir ve yalnızca yetkili servislerin bu bilgilere erişmesine izin verir. Bunuda token aracacılığı ile yapar.

Proje içerisindeki kullanımları.

Core > Consul

ConsulRegistrationService : Servisin consule kayıt olmasını sağladık.
VaultService : Uygulama ayağa kaltığında vault servisten ilgili configlerimizi çekip mevcut configurationsa ekledik.
