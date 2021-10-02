using SimpleCmsWebApi.Data;
using SimpleCmsWebApi.DTO;
using SimpleCmsWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleCmsWebApi.IntegrationTests
{
    public class ArticleControllerTests : IClassFixture<SimpleCmsWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private readonly SimpleCmsWebApplicationFactory<Startup> _factory;
        public const string url = "api/articles";

        public ArticleControllerTests(SimpleCmsWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async void GetArticle_ReturnsExpectedArticle()
        {
            var expectedArticle = DbUtilities.Articles.First();

            var response = await _client.GetAsync($"{url}/{expectedArticle.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var returnedArticle = await response.Content.ReadAsAsync<Article>();
            Assert.Equal(expectedArticle.Id, returnedArticle.Id);
        }

        [Fact]
        public async void GetArticle_NotExistringId_ReturnsNotFounds()
        {
            var response = await _client.GetAsync($"{url}/{150}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void GetArticles_ReturnsArticles()
        {
            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadAsAsync<IEnumerable<Article>>();
            Assert.True(result.Any());
        }

        [Fact]
        public async void PostArticles_WithoutAuthentication_UnauthorizedReturned()
        {
            ArticleUpdateDto newArticleDto = new()
            {
                Body = "New article", 
                Title = "New title"
            };

            var response = await _client.PostAsync<ArticleUpdateDto>($"{url}", newArticleDto, new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void PostArticle_TokenProvided_ArticleCreated()
        {
            ArticleUpdateDto newArticleDto = new()
            {
                Body = "New article",
                Title = "New title"
            };
            AddAuthentication();
            var response = await _client.PostAsync($"{url}", newArticleDto, new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedArticle = await response.Content.ReadAsAsync<Article>();
            Assert.Equal(newArticleDto.Body, returnedArticle.Body);
            Assert.Equal(newArticleDto.Title, returnedArticle.Title);
        }

        [Fact]
        public async void PostArticle_TokenProvided_CreatedTimestampEqualsNow()
        {
            ArticleUpdateDto newArticleDto = new()
            {
                Body = "New article",
                Title = "New title"
            };
            AddAuthentication();
            var currentTime = DateTime.UtcNow;

            var response = await _client.PostAsync($"{url}", newArticleDto, new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedArticle = await response.Content.ReadAsAsync<Article>();
            Assert.True(returnedArticle.Timestamp >= currentTime && returnedArticle.Timestamp <= DateTime.UtcNow);
        }

        [Fact]
        public async void PutArticle_TokenProvided_ArticleUpdated()
        {
            var updatingArticle = DbUtilities.Articles.First();
            ArticleUpdateDto updateDto = new()
            {
                Body = "New article",
                Title = "New title"
            };
            AddAuthentication();
            var currentTime = DateTime.UtcNow;

            var response = await _client.PutAsync($"{url}/{updatingArticle.Id}", updateDto, new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetService<IArticlesRepository>();
            var updatedArticle = repository.GetArticle(updatingArticle.Id);
            Assert.Equal(updateDto.Body, updatedArticle.Body);
            Assert.Equal(updateDto.Title, updatedArticle.Title);
            Assert.True(updatedArticle.Timestamp >= currentTime && updatedArticle.Timestamp <= DateTime.UtcNow);
        }

        //TODO: Add tests for all methods

        private void AddAuthentication()
        {
            _client.DefaultRequestHeaders.Add("SuperToken", "C972A7379C6946C3928D3424638A6D3B");
        }
    }
}
