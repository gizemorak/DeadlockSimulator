# Deadlock Simulator (C#)

Bu proje, bilişim teknolojileri yüksek lisans işletim sistemleri dersi kapsamında işletim sistemlerinde önemli bir problem olan **deadlock (ölümcül kilitlenme)** durumunu simüle etmek ve analiz etmek amacıyla geliştirilmiştir.

## 📌 Proje Amacı

Bu projenin amacı:

* Deadlock oluşumunu göstermek
* Deadlock’un nedenlerini analiz etmek
* Deadlock’u önlemek ve tespit etmek
* Farklı senaryolar altında sistem davranışını incelemektir

---

## ⚙️ Kullanılan Teknolojiler

* C#
* .NET
* Thread (çoklu iş parçacığı)
* lock / Monitor (senkronizasyon)

---

## 🚀 Nasıl Çalıştırılır?

1. Projeyi Visual Studio ile açın
2. Programı çalıştırın
3. Konsolda aşağıdaki menü çıkacaktır:

```
1 - Deadlock Oluşturan Senaryo
2 - Deadlock Önlenmiş Senaryo
3 - Timeout ile Deadlock Tespiti
4 - İstatistiksel Test
```

İlgili senaryoyu seçerek programı çalıştırabilirsiniz.

---

## 🧪 Senaryolar

### 1. Deadlock Oluşturan Senaryo

* Thread-1: Resource-A → Resource-B
* Thread-2: Resource-B → Resource-A
* Sistem kilitlenir (deadlock oluşur)

---

### 2. Deadlock Önlenmiş Senaryo

* Tüm thread’ler aynı sırada kaynak alır (A → B)
* Deadlock oluşmaz
* Program başarıyla tamamlanır

---

### 3. Timeout ile Deadlock Tespiti

* `Monitor.TryEnter()` kullanılır
* Belirli süre içinde kaynak alınamazsa:

  * Deadlock tespit edilir
  * Loglanır
  * Sistem tamamen kilitlenmez

---

### 4. İstatistiksel Test

* Sistem 10 kez çalıştırılır
* Her denemede deadlock olup olmadığı kontrol edilir
* Aşağıdaki değerler hesaplanır:

  * Deadlock sayısı
  * Başarılı işlem sayısı
  * Deadlock oranı (%)
  * Ortalama süre

---

## 📝 Loglama

Tüm olaylar hem konsola hem de dosyaya yazdırılır.

📂 Log dosyası:

```
log.txt
```

İçerik:

* Kaynak alma/bırakma
* Bekleme durumları
* Deadlock tespiti
* Test sonuçları

---

## 🧠 Kullanılan Yaklaşımlar

Bu projede aşağıdaki deadlock yönetim teknikleri uygulanmıştır:

* Deadlock oluşturma (simülasyon)
* Deadlock prevention (kaynak sırası sabitleme)
* Deadlock detection (timeout ile tespit)
* Rastgele bekleme süreleri ile test
* İstatistiksel analiz

---

## 📊 Örnek Çıktı

```
Thread-1: Resource-A alındı
Thread-2: Resource-B alındı
Thread-1: Resource-B bekleniyor
Thread-2: Resource-A bekleniyor
Deadlock detected
```

---

## 📁 Proje Yapısı

```
/src        → Kaynak kodlar
/report     → Rapor dosyası
/output     → Log ve çıktılar
README.md   → Proje açıklaması
```

---

## 📚 Öğrenilenler

Bu proje sayesinde:

* Deadlock’un nasıl oluştuğu
* Coffman koşulları
* Senkronizasyon mekanizmaları
* Deadlock önleme ve tespit yöntemleri

uygulamalı olarak öğrenilmiştir.

---

## 👨‍💻 Geliştirici

Ad Soyad: GİZEM ORAK
Ders: İşletim Sistemleri
Proje: Deadlock Simülatörü
