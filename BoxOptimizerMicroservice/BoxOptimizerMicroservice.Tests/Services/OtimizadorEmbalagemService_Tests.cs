using Moq;
using FluentAssertions;
using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BoxOptimizerMicroservice.Services.EstrategiaOtimizacaoEmbalagem;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Tests.Services
{
    public class OtimizadorEmbalagemService_Tests
    {
        private readonly Mock<IEstrategiaDeEmpacotamento> _mockEstrategia;
        private readonly BoxOptimizerDbContext _dbContextInMemory;
        private readonly OtimizadorEmbalagemService _service;
        private readonly List<Caixa> _listaDeTiposDeCaixaSeed;

        public OtimizadorEmbalagemService_Tests()
        {
            _mockEstrategia = new Mock<IEstrategiaDeEmpacotamento>();

            var options = new DbContextOptionsBuilder<BoxOptimizerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContextInMemory = new BoxOptimizerDbContext(options);

            _listaDeTiposDeCaixaSeed = new List<Caixa>
            {
                new Caixa("Caixa 1", 30, 40, 80),
                new Caixa("Caixa 2", 80, 50, 40),
                new Caixa("Caixa 3", 50, 80, 60)
            };
            _dbContextInMemory.Caixa.AddRange(_listaDeTiposDeCaixaSeed);
            _dbContextInMemory.SaveChanges();

            _service = new OtimizadorEmbalagemService(_mockEstrategia.Object, _dbContextInMemory);
        }

        private RequestPedidosDto CriarRequestGlobalComUmPedido(int pedidoId, List<RequestProdutoDto> produtos)
        {
            return new RequestPedidosDto
            {
                Pedidos = new List<RequestPedidoDto>
                {
                    new RequestPedidoDto 
                    { 
                        PedidoId = pedidoId, 
                        Produtos = produtos 
                    }
                }
            };
        }

        [Fact]
        public async Task Deve_ChamarEstrategiaESalvarResultadoQuandoPedidoValido()
        {
            // Arr
            var produtos = new List<RequestProdutoDto> { new RequestProdutoDto { ProdutoId = "P1" } };
            var requestGlobal = CriarRequestGlobalComUmPedido(1, produtos);

            var resultadoEstrategia = new List<ResponseEmbalagemDto>
            {
                new ResponseEmbalagemDto { 
                    CaixaId = "Caixa 1", 
                    Produtos = new List<string> { "P1" } }
            };
            _mockEstrategia.Setup(s => s.OrganizarProdutosEmCaixas(requestGlobal.Pedidos[0], It.IsAny<List<Caixa>>()))
                           .Returns(resultadoEstrategia);

            // Act
            var responseGlobal = await _service.OtimizarMultiplosPedidosAsync(requestGlobal);

            // Assert
            responseGlobal.Pedidos.Should().HaveCount(1);
            responseGlobal.Pedidos[0].PedidoId.Should().Be(1);
            responseGlobal.Pedidos[0].Caixas.Should().HaveSameCount(resultadoEstrategia);
            responseGlobal.Pedidos[0].Caixas[0].CaixaId.Should().Be("Caixa 1");

            _mockEstrategia.Verify(s => s.OrganizarProdutosEmCaixas(requestGlobal.Pedidos[0], It.IsAny<List<Caixa>>()), Times.Once);

            var resultadosSalvos = await _dbContextInMemory.EmbalagemResultado
                                                 .Where(r => EF.Property<int>(r, "_pedidoId") == 1)
                                                 .ToListAsync();
            resultadosSalvos.Should().HaveCount(1);

            var resultadoPedido1Salvo = resultadosSalvos.FirstOrDefault(); 
            resultadoPedido1Salvo.Should().NotBeNull();
            resultadoPedido1Salvo.GetProdutosNestaCaixa().Should().Contain("P1");
        }

        [Fact]
        public async Task NaoDeve_SalvarNadaQuandoEstrategiaNaoRetornaCaixas()
        {
            // Arr
            var produtos = new List<RequestProdutoDto> { new RequestProdutoDto { ProdutoId = "P1" } };
            var requestGlobal = CriarRequestGlobalComUmPedido(1, produtos);

            _mockEstrategia.Setup(s => s.OrganizarProdutosEmCaixas(requestGlobal.Pedidos[0], It.IsAny<List<Caixa>>()))
                           .Returns(new List<ResponseEmbalagemDto>()); 

            // Act
            var responseGlobal = await _service.OtimizarMultiplosPedidosAsync(requestGlobal);

            // Assert
            responseGlobal.Pedidos[0].Caixas.Should().BeEmpty();

            var resultadosSalvos = await _dbContextInMemory.EmbalagemResultado
                                                 .Where(r => EF.Property<int>(r, "_pedidoId") == 1)
                                                 .ToListAsync(); resultadosSalvos.Should().BeEmpty();
        }

        [Fact]
        public async Task Deve_LancarBadHttpRequestExceptionQuandoRequestGlobalNulo()
        {
            // Arr
            RequestPedidosDto requestGlobal = null;

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _service.OtimizarMultiplosPedidosAsync(requestGlobal));
        }

        [Fact]
        public async Task Deve_LancarBadHttpRequestExceptionQuandoListaDePedidosNula()
        {
            // Arr
            var requestGlobal = new RequestPedidosDto { Pedidos = null };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _service.OtimizarMultiplosPedidosAsync(requestGlobal));
        }

        [Fact]
        public async Task Deve_LancarBadHttpRequestExceptionQuandoListaDePedidosVazia()
        {
            // Arr
            var requestGlobal = new RequestPedidosDto { Pedidos = new List<RequestPedidoDto>() };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _service.OtimizarMultiplosPedidosAsync(requestGlobal));
        }

        [Fact]
        public async Task Deve_LancarBadHttpRequestExceptionQuandoProdutosDoPedidoNulo()
        {
            // Arr
            var requestGlobal = new RequestPedidosDto
            {
                Pedidos = new List<RequestPedidoDto> {
                    new RequestPedidoDto { PedidoId = 1, Produtos = null }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _service.OtimizarMultiplosPedidosAsync(requestGlobal));
        }

        [Fact]
        public async Task Deve_SalvarObservacaoQuandoEstrategiaRetornaProdutoNaoEmbalavel()
        {
            // Arr
            var produtos = new List<RequestProdutoDto> { new RequestProdutoDto { ProdutoId = "ProdX" } };
            var requestGlobal = CriarRequestGlobalComUmPedido(10, produtos);

            var resultadoEstrategia = new List<ResponseEmbalagemDto>
            {
                new ResponseEmbalagemDto 
                { 
                    CaixaId = null, 
                    Produtos = new List<string> { "ProdX" }, Observacao = "Não coube" }
            };
            _mockEstrategia.Setup(s => s.OrganizarProdutosEmCaixas(requestGlobal.Pedidos[0], It.IsAny<List<Caixa>>()))
                           .Returns(resultadoEstrategia);

            // Act
            await _service.OtimizarMultiplosPedidosAsync(requestGlobal);

            // Assert
            var resultadoSalvo = await _dbContextInMemory.EmbalagemResultado
                                                 .FirstOrDefaultAsync(r => EF.Property<int>(r, "_pedidoId") == 10);
            resultadoSalvo.Should().NotBeNull();
            resultadoSalvo.GetObservacao().Should().Be("Não coube");
            resultadoSalvo.GetTipoCaixaUsadaId().Should().BeNull();
        }
    }
}