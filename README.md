# 💡 HabitsApp Backend

Bu proje, kullanıcıların alışkanlıklarını oluşturabildiği, takip edebildiği ve istatistiksel verilerle desteklenen bir **alışkanlık takip sisteminin sunucu tarafını** oluşturur. Modern yazılım geliştirme prensipleriyle tasarlanan HabitsApp Backend, **.NET 9** ve **Clean Architecture** mimarisi ile yapılandırılmıştır. Ayrıca, admin paneli, blog ve pomodoro gibi ek modüllerle kolayca genişletilebilir bir yapıya sahiptir.

---

## 🏗️ Mimari Yapı: Clean Architecture

HabitsApp Backend, yazılım kalitesini ve sürdürülebilirliği en üst düzeye çıkarmak amacıyla **Clean Architecture** prensiplerine göre yapılandırılmıştır. Bu mimari yaklaşım, katmanlar arası sıkı bağımlılıkları ortadan kaldırarak kodun daha **okunabilir, test edilebilir ve esnek** olmasını sağlar. Her katman yalnızca bir alt katmanı tanır ve tüm bağımlılıklar merkeze (domain katmanına) doğrudur.

---

## ⚙️ Kullanılan Temel Teknolojiler

Projede kullanılan başlıca teknolojiler ve kütüphaneler aşağıdaki tabloda listelenmiştir:

| Amaç                     | Teknoloji/Kütüphane                                 | Açıklama                                                              |
| :----------------------- | :-------------------------------------------------- | :-------------------------------------------------------------------- |
| **API Katmanı** | Minimal API, [Scalar](https://scalar.com/)          | Hafif ve hızlı API geliştirmesi; dinamik API dokümantasyonu ve keşfi. |
| **CQRS & Validasyon** | MediatR, AutoMapper, FluentValidation               | Komut ve sorgu ayrımı; nesneler arası otomatik eşleme; güçlü veri doğrulama. |
| **Veri Erişimi** | Entity Framework Core, Repository, Unit of Work     | Veritabanı işlemleri için ORM; temiz ve test edilebilir veri erişim katmanı. |
| **Kimlik Doğrulama** | JWT Bearer Token                                    | Güvenli ve standart tabanlı kullanıcı kimlik doğrulama ve yetkilendirme. |
| **Ortak Yapılar** | Result Pattern (başarı/hata modeli)                 | API yanıtlarında tutarlı başarı ve hata yönetimi modeli.             |

---

## 🎯 Temel Özellikler

HabitsApp Backend, alışkanlık takip sisteminin ihtiyaç duyduğu temel ve gelişmiş özellikleri bünyesinde barındırır:

* 🔐 **Güçlü Kimlik Doğrulama ve Yetkilendirme:** JWT (JSON Web Token) tabanlı güvenli kimlik doğrulama ve yetkilendirme mekanizması.
* 🧠 **CQRS ile Performanslı İşlemler:** MediatR kütüphanesiyle implemente edilen Command Query Responsibility Segregation (CQRS) yapısı sayesinde sorgu ve komut işlemleri ayrılarak performans ve ölçeklenebilirlik artırılmıştır.
* ✅ **Kapsamlı Model Doğrulama:** FluentValidation ile güçlü ve esnek model doğrulama kuralları tanımlanarak veri bütünlüğü sağlanmıştır.
* 📊 **Kullanıcı Alışkanlık Yönetimi ve İstatistik Takibi:** Kullanıcıların alışkanlıklarını oluşturma, düzenleme, silme ve detaylı istatistiklerini takip etme imkanı.
* 🧩 **Standart API Yanıtları:** Result Pattern kullanılarak tüm API yanıtları tutarlı bir başarı/hata modeliyle dönülür, bu da istemci tarafında hata yönetimini kolaylaştırır.
* ⚡ **Dinamik API Dokümantasyonu ve Keşfi:** Scalar ile entegre edilen dinamik GraphQL UI sayesinde API endpoint'leri kolayca keşfedilebilir ve test edilebilir.
* 📁 **Katmanlı, Sürdürülebilir ve Test Edilebilir Yapı:** Clean Architecture sayesinde projenin her katmanı birbirinden bağımsız olup kolayca test edilebilir ve uzun vadede sürdürülebilirliği garantiler.

---


### Gereksinimler

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) yüklü olmalı.
* Veritabanı olarak **SQL Server** veya **PostgreSQL** (tercihinize göre) kurulu olmalı.
* (Opsiyonel) Eğer Docker kullanmak isterseniz **Docker** ve **Docker Compose** kurulu olmalı.



## 🤝 Katkıda Bulunma

Projeye katkıda bulunmak ister misiniz? Harika! Lütfen bir Pull Request göndermeden önce mevcut sorunlara (issues) göz atın veya yeni bir özellik önerisi için issue açmaktan çekinmeyin. Kod standartlarına uyarak ve açıklayıcı commit mesajları kullanarak katkılarınızın daha hızlı incelenmesine yardımcı olabilirsiniz.

---




## 📞 İletişim

Herhangi bir sorunuz veya öneriniz olursa, lütfen issues bölümünü kullanmaktan çekinmeyin veya [deniz.furkann@outlook.com](mailto:deniz.furkann@outlook.com) adresinden iletişime geçin.
