using BookDemo.Application.DTOs;
using BookDemo.Domain.Entities;

namespace BookDemo.Application.Contracts
{
    /// <summary>
    /// IBookServiceBase defines business-level operations
    /// related to Book entities.
    ///
    /// ❗ This interface:
    /// - Does NOT know about Entity Framework
    /// - Does NOT expose Repository or DbContext
    /// - Represents application/business language
    /// Version-specific list operations are defined in V1/IBookService and V2/IBookService.
    /// </summary>
    /// 
    /// /// IBookService yazarken dikkat edeceğin 7 kural:
    ///
    /// 1. Controller'ın kullandığı use-case'leri expose et
    ///    "DB'de ne var?" değil, "kullanıcı ne yapmak istiyor?" diye düşün.
    ///
    /// 2. Entity'yi (Book) dışarı taşımamaya çalış
    ///    API/Controller genelde DTO ile konuşmalı.
    ///
    /// 3. Tracking gibi EF Core detayları Service dışına sızmasın
    ///    trackChanges parametresini controller'a taşıma.
    ///    Service içinde karar ver: read için false, update için true.
    ///
    /// 4. Save / Transaction sınırını service belirlesin
    ///    Bir use-case içinde birden fazla repo çağrısı olabilir → en sonunda Save().
    ///
    /// 5. Validasyon ve iş kuralları service'te
    ///    Örn: fiyat negatif olamaz, title boş olamaz, aynı title tekrar eklenemez vb.
    ///
    /// 6. Return tipi API dostu olsun
    ///    bool/void yerine "ne oldu?"yu ifade et.
    ///    En azından BookDto, IEnumerable BookDto, vs.
    ///
    /// 7. İsimlendirme CRUD değil use-case odaklı olsun
    ///    GetAllBooks yerine GetBooks (liste ekranı)
    ///    GetBookById yerine GetBook veya GetBookDetails
    /// </remarks>
    public interface IBookServiceBase
    {
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(BookForCreationDto book);
        Task UpdateBookAsync(int id, BookForUpdateDto book);
        Task DeleteBookAsync(int id);
        Task<(BookForUpdateDto bookToPatch, Book bookEntity)> GetBookForPatchAsync(int id);
        Task SaveChangesForPatchAsync(BookForUpdateDto bookToPatch, Book bookEntity);

    }
}
