using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using SimpleCmsWebApi.Controllers;
using SimpleCmsWebApi.Data;
using SimpleCmsWebApi.DTO;
using SimpleCmsWebApi.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleCmsWebApi.UnitTests
{
    public class AtriclesControllerTests
    {
        [Fact]
        public void GetArticle_NotExistingIdPassed_ReturnsNotFound()
        {
            int notExistingId = 10;
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == notExistingId))).Returns(() => null);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.GetArticle(notExistingId);

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public void GetArticle_ExistingIdPassed_ReturnsExpectedDto()
        {
            ArticleDto expectedDto = new() { Id = 10 };
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == expectedDto.Id))).Returns(() => new Article());
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            mapper.Setup(m => m.Map<ArticleDto>(It.IsNotNull<Article>())).Returns(() => expectedDto);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.GetArticle(expectedDto.Id);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            OkObjectResult result = (OkObjectResult)actionResult.Result;

            Assert.Equal(expectedDto, result.Value);
        }

        [Fact]
        public void GetArticles_ReturnsExpectedCollection()
        {
            IEnumerable<ArticleDto> expectedResult = new List<ArticleDto>()
            {
                new ArticleDto()
                {
                    Id = 1,
                    Title = "Article 1",
                    Body = "Article body 1",
                    Timestamp = new DateTime(2020, 12, 20, 12, 00, 32, DateTimeKind.Utc)
                },
                new ArticleDto()
                {
                    Id = 2,
                    Title = "Article 2",
                    Body = "Article body 2",
                    Timestamp = new DateTime(2012, 9, 1, 9, 05, 00, DateTimeKind.Utc)
                },
                new ArticleDto()
                {
                    Id = 3,
                    Title = "Article 3",
                    Body = "Article body 3",
                    Timestamp = new DateTime(2021, 3, 22, 10, 12, 54, DateTimeKind.Utc)
                }
            };

            string sort = "Id";
            int? offset = 1;
            int? limit = 10;
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticles(
                    It.Is<string>((s) => s == sort),
                    It.Is<int?>(o => o == offset),
                    It.Is<int?>(l => l == limit)))
                .Returns(() => new List<Article>());
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            mapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(It.IsNotNull<IEnumerable<Article>>()))
                .Returns(() => expectedResult);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.GetArticles(sort, offset, limit);

            Assert.IsType<OkObjectResult>(actionResult.Result);
            OkObjectResult result = (OkObjectResult)actionResult.Result;
            Assert.Equal(expectedResult, result.Value);
        }

        [Fact]
        public void CreateArticle_ArticleSaved()
        {
            ArticleUpdateDto createDto = new()
            {
                Body = "Body",
                Title = "Title"
            };

            Article newArticle = new() { Id = 1 };

            ArticleDto expectedDto = new()
            {
                Id = 1,
                Body = "Body",
                Title = "Title"
            };
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);

            mapper.Setup(m => m.Map<Article>(It.Is<ArticleUpdateDto>(dto => dto == createDto)))
                .Returns(newArticle);

            mapper.Setup(m => m.Map<ArticleDto>(It.Is<Article>(article => article == newArticle)))
                .Returns(expectedDto);
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.CreateArticle(It.Is<Article>(article => article == newArticle)));
            repository.Setup(r => r.SaveChanges()).Returns(true);

            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.CreateArticle(createDto);

            repository.Verify(r => r.SaveChanges(), Times.Once);
            Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
        }

        [Fact]
        public void DeleteArticle_NotExistingIdPassed_ReturnsNotFound()
        {
            int notExistingId = 10;
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == notExistingId))).Returns(() => null);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.DeleteArticle(notExistingId);

            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void DeleteArticle_ExistingIdPassed_ArticleDeleted()
        {
            Article articleToDelete = new() { Id = 1 };
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == articleToDelete.Id)))
                .Returns(() => articleToDelete);
            repository.Setup(r => r.DeleteArticle(It.Is<Article>(article => article == articleToDelete)));
            repository.Setup(r => r.SaveChanges()).Returns(true);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);

            ArticlesController controller = new(repository.Object, mapper.Object);

            controller.DeleteArticle(articleToDelete.Id);

            repository.Verify(r => r.DeleteArticle(It.Is<Article>(article => article == articleToDelete)), Times.Once);
            repository.Verify(r => r.SaveChanges(), Times.Once);
            var actionResult = controller.DeleteArticle(articleToDelete.Id);
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public void PutArticle_NotExistingIdPassed_ReturnsNotFound()
        {
            int notExistingId = 10;
            ArticleUpdateDto updateDto = new()
            {
                Body = "Body",
                Title = "Title"
            };

            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == notExistingId))).Returns(() => null);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.PutArticle(notExistingId, updateDto);

            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void PutArticle_NotExistingIdPassed_ArticleUpdated()
        {
            Article articleToUpdate = new() { Id = 1 };
            ArticleUpdateDto updateDto = new()
            {
                Body = "Body",
                Title = "Title"
            };

            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.SaveChanges()).Returns(true);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == articleToUpdate.Id)))
                .Returns(() => articleToUpdate);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            mapper.Setup(m => m.Map(It.Is<ArticleUpdateDto>((dto) => dto == updateDto), It.Is<Article>(article => article == articleToUpdate)))
                .Returns(articleToUpdate);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.PutArticle(articleToUpdate.Id, updateDto);

            repository.Verify(r => r.SaveChanges(), Times.Once);
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public void PatchArticle_NotPatchPassed_ReturnsBadRequest()
        {
            int notExistingId = 10;

            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.PatchArticle(notExistingId, null);

            Assert.IsType<BadRequestResult>(actionResult);
        }

        [Fact]
        public void PatchArticle_NotExistingIdPassed_ReturnsNotFound()
        {
            int notExistingId = 10;
            ArticleUpdateDto updateDto = new()
            {
                Body = "Body",
                Title = "Title"
            };
            Mock<JsonPatchDocument<ArticleUpdateDto>> jsonPatch = new(MockBehavior.Strict);
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == notExistingId))).Returns(() => null);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            ArticlesController controller = new(repository.Object, mapper.Object);

            var actionResult = controller.PatchArticle(notExistingId, jsonPatch.Object);

            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void PatchArticle_ArticlePatched()
        {
            Article articleToUpdate = new() { Id = 1 };
            ArticleUpdateDto updateDto = new()
            {
                Body = "Body",
                Title = "Title"
            };

            Mock<JsonPatchDocument<ArticleUpdateDto>> jsonPatch = new();
            //Repo
            Mock<IArticlesRepository> repository = new(MockBehavior.Strict);
            repository.Setup(r => r.GetArticle(It.Is<int>(id => id == articleToUpdate.Id))).Returns(() => articleToUpdate);
            repository.Setup(r => r.SaveChanges()).Returns(true);
            //Mapping
            Mock<IMapper> mapper = new(MockBehavior.Strict);
            mapper.Setup(m => m.Map<ArticleUpdateDto>(It.Is<Article>(article => article == articleToUpdate)))
                .Returns(updateDto);
            mapper.Setup(m => m.Map(It.Is<ArticleUpdateDto>((dto) => dto == updateDto), It.Is<Article>(article => article == articleToUpdate)))
                .Returns(articleToUpdate);
            //Validator
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));

            ArticlesController controller = new(repository.Object, mapper.Object);
            controller.ObjectValidator = objectValidator.Object;

            var actionResult = controller.PatchArticle(articleToUpdate.Id, jsonPatch.Object);

            repository.Verify(r => r.SaveChanges(), Times.Once);
            Assert.IsType<NoContentResult>(actionResult);
        }
    }
}
