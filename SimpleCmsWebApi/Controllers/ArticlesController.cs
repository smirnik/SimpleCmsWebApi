using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SimpleCmsWebApi.Data;
using SimpleCmsWebApi.DTO;
using SimpleCmsWebApi.Models;

namespace SimpleCmsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesRepository _repository;
        private readonly IMapper _mapper;

        public ArticlesController(IArticlesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns article by id
        /// </summary>
        /// <param name="id">Article id</param>
        [HttpGet("{id}", Name = nameof(GetArticle))]
        [AllowAnonymous]
        public ActionResult<ArticleDto> GetArticle(int id)
        {
            var article = _repository.GetArticle(id);
            if (article != null)
            {
                return Ok(_mapper.Map<ArticleDto>(article));
            }

            return NotFound();
        }


        ///<summary>
        ///Returns articles according to passed parameters
        ///</summary>
        ///<remarks>
        ///Example: api/articles?sort=timestamp desc,title&amp;offset=0&amp;limit=10
        ///</remarks>
        ///<param name="limit">Maximum number of items to return</param>
        ///<param name="sort">Sort articles by property name. Multiple properties can be passed separated by commas. To sort in descending order, add 'desc' after property name</param>
        ///<param name="offset">Return articles from a specific number</param>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<ArticleDto> GetArticles([FromQuery] string sort, [FromQuery] int? offset, [FromQuery] int? limit)
        {
            var articles = _repository.GetArticles(sort, offset, limit);
            return Ok(_mapper.Map<IEnumerable<ArticleDto>>(articles));
        }

        /// <summary>
        /// Create article
        /// </summary>
        /// <param name="articleCreateDto">Article to create</param>
        [HttpPost]
        public ActionResult<ArticleDto> CreateArticle(ArticleUpdateDto articleCreateDto)
        {
            var article = _mapper.Map<Article>(articleCreateDto);
            _repository.CreateArticle(article);
            _repository.SaveChanges();

            var articleDto = _mapper.Map<ArticleDto>(article);

            return CreatedAtRoute(nameof(GetArticle), new { id = articleDto.Id }, articleDto);
        }

        /// <summary>
        /// Delete specified article
        /// </summary>
        /// <param name="id">Article id</param>
        [HttpDelete("{id}")]
        public ActionResult DeleteArticle(int id)
        {
            var article = _repository.GetArticle(id);
            if (article == null)
            {
                return NotFound();
            }

            _repository.DeleteArticle(article);
            _repository.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Update article
        /// </summary>
        /// <param name="id">Updating article id</param>
        /// <param name="articleUpdateDto">Update for article</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult PutArticle(int id, ArticleUpdateDto articleUpdateDto)
        {
            var article = _repository.GetArticle(id);
            if (article == null)
            {
                return NotFound();
            }

            _mapper.Map(articleUpdateDto, article);
            _repository.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Partially update specified article
        /// </summary>
        /// <param name="id">Article id</param>
        /// <param name="patch">Patch info</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public ActionResult PatchArticle(int id, JsonPatchDocument<ArticleUpdateDto> patch)
        {
            if (patch == null)
            {
                return BadRequest();
            }

            var article = _repository.GetArticle(id);
            if (article == null)
            {
                return NotFound();
            }
            
            var articleDto = _mapper.Map<ArticleUpdateDto>(article);
            patch.ApplyTo(articleDto, ModelState);
            if (!TryValidateModel(articleDto))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(articleDto, article);
            _repository.SaveChanges();

            return NoContent();
        }
    }
}