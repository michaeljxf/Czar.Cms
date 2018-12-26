
using Czar.Cms.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Linq;
using Czar.Cms.IRepository;
using Czar.Cms.Repository.SqlServer;
using Czar.Cms.Core.Models;
using Czar.Cms.Core.CodeGenerator;
using Czar.Cms.Core.Options;

namespace Czar.Cms.Test
{
    /// <summary>
    /// yilezhu
    /// 2018.12.12
    /// ���Դ���������
    /// ��ʱֻʵ����SqlServer��ʵ���������
    /// </summary>
    public class GeneratorTest
    {
        [Fact]
        public void GeneratorModelForSqlServer()
        {
            var serviceProvider = BuildServiceForSqlServer();
            var codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.GenerateTemplateCodesFromDatabase(true);
            Assert.Equal("SQLServer", DatabaseType.SqlServer.ToString(), ignoreCase: true);

        }

        [Fact]
        public void TestBaseFactory()
        {
            IServiceProvider serviceProvider = BuildServiceForSqlServer();
            IArticleCategoryRepository categoryRepository = serviceProvider.GetService<IArticleCategoryRepository>();
            var category = new ArticleCategory
            {
                Title = "���",
                ParentId = 0,
                ClassList = "",
                ClassLayer = 0,
                Sort = 0,
                ImageUrl = "",
                SeoTitle = "��ʵ�SEOTitle",
                SeoKeywords = "��ʵ�SeoKeywords",
                SeoDescription = "��ʵ�SeoDescription",
                IsDeleted = false,
            };
            var categoryId = categoryRepository.Insert(category);
            var list = categoryRepository.GetList();
            Assert.True(1 == list.Count());
            Assert.Equal("���", list.FirstOrDefault().Title);
            Assert.Equal("SQLServer", DatabaseType.SqlServer.ToString(), ignoreCase: true);
            categoryRepository.Delete(categoryId.Value);
            var count = categoryRepository.RecordCount();
            Assert.True(0 == count);
        }

        /// <summary>
        /// ��������ע��������Ȼ�������
        /// </summary>
        /// <returns></returns>
        public IServiceProvider BuildServiceForSqlServer()
        {
            var services = new ServiceCollection();
            services.Configure<CodeGenerateOption>(options =>
            {
                options.ConnectionString = "Data Source=.;Initial Catalog=CzarCms;User ID=sa;Password=1;Persist Security Info=True;Max Pool Size=50;Min Pool Size=0;Connection Lifetime=300;";
                options.DbType = DatabaseType.SqlServer.ToString();//���ݿ�������SqlServer,�����������Ͳ���ö��DatabaseType
                options.Author = "yilezhu";//��������
                options.OutputPath = "C:\\CzarCmsCodeGenerator";//ģ��������ɵ�·��
                options.ModelsNamespace = "Czar.Cms.Models";//ʵ�������ռ�
                options.IRepositoryNamespace = "Czar.Cms.IRepository";//�ִ��ӿ������ռ�
                options.RepositoryNamespace = "Czar.Cms.Repository.SqlServer";//�ִ������ռ�

            });
            services.AddSingleton<CodeGenerator>();//ע��Model����������
            services.Configure<DbOpion>("CzarCms", GetConfiguration().GetSection("DbOpion"));
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleCategoryRepository, ArticleCategoryRepository>();
            return services.BuildServiceProvider(); //���������ṩ����
        }

        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
