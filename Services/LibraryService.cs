using Grpc.Core;
using LibrarySystem;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using static Google.Rpc.Context.AttributeContext.Types;

namespace LibrarySystem.Services
{
    public class LibraryService : Library.LibraryBase
    {
        private readonly LibraryDbContext _dbContext;
        public LibraryService(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task<CreateBookResponse> CreateBook(CreateBookRequest request, ServerCallContext context)
        {
            var newBook = new Book
            {
                title = request.Title,
                author = request.Author,
                isbn = request.Isbn,
                publication_year = request.PublicationYear,
                genre = request.Genre
            };
            _dbContext.Books.Add(newBook);
            await _dbContext.SaveChangesAsync();

            return await  Task.FromResult(new CreateBookResponse
            {
                Id = newBook.id
            });
        }

        public override async Task<GetBookResponse> GetBook(GetBookRequest request, ServerCallContext context)
        {
            var book = await getBookAndCheckIfExists(request.Id);

            return await Task.FromResult(new GetBookResponse
            {
                Id = book.id,
                Title = book.title,
                Author = book.author,
                Isbn = book.isbn,
                PublicationYear = book.publication_year,
                Genre = book.genre,
                Available = book.available
            });
        }

        public override async Task<ListBooksResponse> ListBooks(ListBooksRequest request, ServerCallContext context)
        {
            var books = new ListBooksResponse();
            var dbBooks = await _dbContext.Books.Select(book => new GetBookResponse
            {
                Id = book.id,
                Title = book.title,
                Author = book.author,
                Isbn = book.isbn,
                PublicationYear = book.publication_year,
                Genre = book.genre,
                Available = book.available
            }).ToListAsync();
            books.Book.AddRange(dbBooks);

            return await Task.FromResult(books);
        }

        public override async Task<UpdateBookResponse> UpdateBook(UpdateBookRequest request, ServerCallContext context)
        {
            var book = await getBookAndCheckIfExists(request.Id);
            book.title = request.Title;
            book.author = request.Author;
            book.isbn = request.Isbn;
            book.publication_year = request.PublicationYear;
            book.genre = request.Genre;
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateBookResponse
            {
                Success = true
            });
        }

        public override async Task<DeleteBookResponse> DeleteBook(DeleteBookRequest request, ServerCallContext context)
        {
            var book = await getBookAndCheckIfExists(request.Id);
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteBookResponse
            {
                Success = true
            });
        }

        private async Task<Book> getBookAndCheckIfExists(int id)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.id == id);
            if (book is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No Book With Id: {id}"));
            }
            return book;
        }

// -----------------------------------------------------------------------------------------------------------------
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var newUser = new User
            {
                name = request.Name,
                email = request.Email
            };
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateUserResponse
            {
                Id = newUser.id
            });
        }

        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var user = await getUserAndCheckIfExists(request.Id);

            return await Task.FromResult(new GetUserResponse
            {
                Id = user.id,
                Name = user.name,
                Email = user.email
            });
        }

        public override async Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
        {
            var users = new ListUsersResponse();
            var dbUsers = await _dbContext.Users.Select(user => new GetUserResponse
            {
                Id = user.id,
                Name = user.name,
                Email = user.email
            }).ToListAsync();
            users.User.AddRange(dbUsers);

            return await Task.FromResult(users);
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var user = await getUserAndCheckIfExists(request.Id);
            user.name = request.Name;
            user.email = request.Email;
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateUserResponse
            {
                Success = true
            });
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var user = await getUserAndCheckIfExists(request.Id);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteUserResponse
            {
                Success = true
            });
        }

        private async Task<User> getUserAndCheckIfExists(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(b => b.id == id);
            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No User With Id: {id}"));
            }
            return user;
        }

// -----------------------------------------------------------------------------------------------------------------
        
        public override async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, ServerCallContext context)
        {
            var user = await getUserAndCheckIfExists(request.UserId);
            var book = await getBookAndCheckIfExists(request.BookId);
            if (!book.available)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, $"Book with Id: {book.id} is Unavailable"));
            }

            book.borrowerId = user.id;
            book.available = false;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new BorrowBookResponse
            {
                Success = true
            });
        }

        public override async Task<ReturnBookResponse> ReturnBook(ReturnBookRequest request, ServerCallContext context)
        {
            var user = await getUserAndCheckIfExists(request.UserId);
            var book = await getBookAndCheckIfExists(request.BookId);
            if (book.available)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Book with Id: {book.id} is Available"));
            }else if (user.id != book.borrowerId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"User with id: {user.id} Didn't Borrow the Book with id: {book.id}"));
            }

            book.available = true;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new ReturnBookResponse
            {
                Success = true
            });
        }

    }
}