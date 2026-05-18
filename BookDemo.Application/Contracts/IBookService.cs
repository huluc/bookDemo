using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    /*
    IBookService yazarken dikkat edeceğin 7 kural

1. Controller’ın kullandığı use-case’leri expose et

“DB’de ne var?” değil, “kullanıcı ne yapmak istiyor?” diye düşün.

2. Entity’yi (Book) dışarı taşımamaya çalış

API/Controller genelde DTO ile konuşmalı.

(DTO = Data Transfer Object: API’den giren/çıkan basit model.)

3. Tracking gibi EF Core detayları Service dışına sızmasın

trackChanges parametresini controller’a taşıma.

Service içinde karar ver: read için false, update için true.

4. Save / Transaction sınırını service belirlesin

Bir use-case içinde birden fazla repo çağrısı olabilir → en sonunda Save().

5. Validasyon ve iş kuralları service’te

Örn: fiyat negatif olamaz, title boş olamaz, aynı title tekrar eklenemez vb.

6. Return tipi API dostu olsun

bool/void yerine “ne oldu?”yu ifade et.

En azından BookDto, IEnumerable<BookDto>, vs.

7. İsimlendirme CRUD değil use-case odaklı olsun

GetAllBooks yerine GetBooks (liste ekranı)

GetBookById yerine GetBook veya GetBookDetails*/
    /// <summary>
    /// IBookService defines business-level operations
    /// related to Book entities.
    ///
    /// ❗ This interface:
    /// - Does NOT know about Entity Framework
    /// - Does NOT expose Repository or DbContext
    /// - Represents application/business language
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Returns all books (read-only).
        /// Tracking is disabled internally for performance.
        /// </summary>
        Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters);
        Task<BookDto> GetBookByIdAsync(int id);

        /// <summary>
        /// Creates a new Book entity and persists it.
        /// Throws exception if input is invalid.
        /// </summary>
        Task<BookDto> CreateBookAsync(BookForCreationDto book);

        /// <summary>
        /// Updates an existing book.
        /// Returns false if the book does not exist.
        /// </summary>
        Task UpdateBookAsync(int id, BookForUpdateDto book);

        /// <summary>
        /// Deletes a book by its identifier.
        /// Returns false if the book does not exist.
        /// </summary>
        Task DeleteBookAsync(int id);
        Task<(BookForUpdateDto bookToPatch, Book bookEntity)> GetBookForPatchAsync(int id);
        Task SaveChangesForPatchAsync(BookForUpdateDto bookToPatch, Book bookEntity);
    }
}
