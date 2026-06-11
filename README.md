# Lexicon Latina

Türkçe kelimelerden Latince karşılıklarını bulan Windows masaüstü sözlük uygulaması.

---

## Ekran Görüntüleri

| Ana Ekran | Sözlük |
| :---: | :---: |
| ![Ana Ekran](images/1.png) | ![Sözlük](images/2.png) |

| Favoriler | Geçmiş |
| :---: | :---: |
| ![Favoriler](images/3.png) | ![Geçmiş](images/4.png) |

---

## Nasıl Çalışır?

1. Kullanıcı Türkçe bir kelime girer.
2. Uygulama, kelimeyi Google Translate API'si üzerinden İngilizceye çevirir.
3. Çevrilen İngilizce kelime Google Translate Sözlük (Single/GTX) API'sine gönderilir ve eşleşen Latince kelimeler, türleri (isim, fiil, sıfat, zarf, bağlaç, edat, deyim) ve anlamlarıyla birlikte listelenir.
4. Kullanıcı, kelime kartlarını genişleterek Tatoeba API'si üzerinden ilgili Latince örnek cümleleri ve çevirilerini (öncelikle Türkçe, yoksa İngilizce) yükleyebilir.
5. Sonuçlar, örnek cümleler ve arama geçmişi yerel olarak saklanır. Kelimeler ve cümleler ayrı ayrı favorilere eklenebilir.

---

## Özellikler

- Türkçe → İngilizce → Latince çift aşamalı akıllı arama
- Kelime türü (isim, fiil, sıfat, zarf, bağlaç, edat, deyim) ve kısa anlam bilgisiyle detaylı sonuçlar
- Latince kelimeler için Tatoeba API destekli örnek cümleler (öncelikli Türkçe veya İngilizce çevirileriyle)
- Google Translate TTS entegrasyonu ile kelimeler ve örnek cümleler için sesli telaffuz (okunuşu dinleme)
- Kelimeleri veya örnek cümleleri panoya hızlıca kopyalama
- Ayrı sekmelerde yönetilebilen Kelime ve Cümle Favorileri (kalıcı, `.json` formatında yerel kayıt)
- Son 50 aramayı kaydeden ve hızlıca tekrar aramayı sağlayan arama geçmişi
- Koyu tema ve özelleştirilmiş modern pencere tasarımı

---

## Teknoloji

- .NET 10.0 / WPF
- MVVM mimarisi
- Google Translate Single/GTX & TTS API'leri (kayıtsız, ücretsiz endpoint)
- [Tatoeba API](https://tatoeba.org/) (örnek cümleler için)

---

## Kurulum

[Releases](https://github.com/mirmehmet/lexicon-latina/releases) sayfasından `.exe` dosyasını indirip çalıştırın. Kurulum gerektirmez.

Derlemek için:

```bash
git clone https://github.com/mirmehmet/lexicon-latina.git
cd lexicon-latina
dotnet run
```
---

*Made with curiosity by [Mir](https://github.com/mirmehmet)*
