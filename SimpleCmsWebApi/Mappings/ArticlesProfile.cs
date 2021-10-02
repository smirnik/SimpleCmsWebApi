using AutoMapper;
using SimpleCmsWebApi.DTO;
using SimpleCmsWebApi.Models;

namespace SimpleCmsWebApi.Mappings
{
    public class ArticlesProfile : Profile
    {
        public ArticlesProfile()
        {
            CreateMap<Article, ArticleDto>();
            CreateMap<ArticleUpdateDto, Article>();
            CreateMap<Article, ArticleUpdateDto>();
        }
    }
}