using FluentAssertions;
using BoxOptimizerMicroservice.Entities;
using BoxOptimizerMicroservice.Services;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem.Dtos;

namespace BoxOptimizerMicroservice.Tests.Services
{
    public class EstrategiaEmpacotamentoEco_Tests
    {
        private readonly EstrategiaEmpacotamentoEco _estrategia;
        private readonly List<Caixa> _caixasDisponiveisPadrao;

        public EstrategiaEmpacotamentoEco_Tests()
        {
            _estrategia = new EstrategiaEmpacotamentoEco();
            _caixasDisponiveisPadrao = new List<Caixa>
            {
                new Caixa("Caixa 1", 30, 40, 80),
                new Caixa("Caixa 2", 80, 50, 40),
                new Caixa("Caixa 3", 50, 80, 60)
            }.OrderBy(c => c.Volume).ToList();
        }

        [Fact]
        public void Deve_EmbalarProdutoUnicoPequenoNaCaixaUmQuandoProdutoCabe()
        {
            var pedido = new RequestPedidoDto
            {
                PedidoId = 1,
                Produtos = new List<RequestProdutoDto>
                {
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "ProdPequeno", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 10, 
                            Largura = 10, 
                            Comprimento = 10 
                        } 
                    }
                }
            };

            var resultado = _estrategia.OrganizarProdutosEmCaixas(pedido, _caixasDisponiveisPadrao);

            resultado.Should().NotBeNull().And.HaveCount(1);
            resultado[0].CaixaId.Should().Be("Caixa 1");
            resultado[0].Produtos.Should().Contain("ProdPequeno");
        }

        [Fact]
        public void Deve_MarcarProdutoComoNaoEmbalavelQuandoNaoCabeEmNenhumaCaixa()
        {
            var pedido = new RequestPedidoDto
            {
                PedidoId = 2,
                Produtos = new List<RequestProdutoDto>
                {
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "ProdGigante", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 100, 
                            Largura = 100, 
                            Comprimento = 100 
                        } 
                    }
                }
            };

            var resultado = _estrategia.OrganizarProdutosEmCaixas(pedido, _caixasDisponiveisPadrao);

            resultado.Should().NotBeNull().And.HaveCount(1);
            resultado[0].CaixaId.Should().BeNull();
            resultado[0].Observacao.Should().Be("Produto não cabe em nenhuma caixa disponível.");
            resultado[0].Produtos.Should().Contain("ProdGigante");
        }

        [Fact]
        public void Deve_AgruparDoisProdutosPequenosNaCaixaUmQuandoAmbosCabemIndividualmente()
        {
            var pedido = new RequestPedidoDto
            {
                PedidoId = 3,
                Produtos = new List<RequestProdutoDto>
                {
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "ItemA", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 5, 
                            Largura = 5, 
                            Comprimento = 5 
                        } 
                    },
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "ItemB", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 6, 
                            Largura = 6, 
                            Comprimento = 6 
                        } 
                    }
                }
            };

            var resultado = _estrategia.OrganizarProdutosEmCaixas(pedido, _caixasDisponiveisPadrao);

            resultado.Should().HaveCount(1);
            resultado[0].CaixaId.Should().Be("Caixa 1");
            resultado[0].Produtos.Should().HaveCount(2).And.Contain(new[] { "ItemA", "ItemB" });
        }

        [Fact]
        public void Deve_ColocarTodosEmCaixaUmQuandoEquipamentoAudioEMenoresCabem()
        {
            var pedido = new RequestPedidoDto
            {
                PedidoId = 21,
                Produtos = new List<RequestProdutoDto>
                {
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "EquipamentoAudio", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 40, 
                            Largura = 45, 
                            Comprimento = 30 
                        } 
                    },
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "MicrofoneMesa", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 20, 
                            Largura = 8, 
                            Comprimento = 8 
                        } 
                    },
                    new RequestProdutoDto 
                    { 
                        ProdutoId = "CaboXLR", 
                        Dimensoes = new RequestDimensaoDto 
                        { 
                            Altura = 5, 
                            Largura = 15, 
                            Comprimento = 15 
                        } 
                    }
                }
            };

            var resultado = _estrategia.OrganizarProdutosEmCaixas(pedido, _caixasDisponiveisPadrao);

            resultado.Should().HaveCount(1);
            var caixaUnica = resultado.First();
            caixaUnica.CaixaId.Should().Be("Caixa 1");
            caixaUnica.Produtos.Should().Contain("EquipamentoAudio")
                .And.Contain("MicrofoneMesa")
                .And.Contain("CaboXLR");
        }

        [Fact]
        public void Deve_ColocarTodosNaCaixaUmQuandoTodosCabemNelaAposRotacao()
        {
            // Arr
            var pedido = new RequestPedidoDto
            {
                PedidoId = 22,
                Produtos = new List<RequestProdutoDto>
                {
                    new RequestProdutoDto
                    {
                        ProdutoId = "EquipamentoAudioGrande",
                        Dimensoes = new RequestDimensaoDto { Altura = 70, Largura = 40, Comprimento = 30 } 
                    },
                    new RequestProdutoDto
                    {
                        ProdutoId = "PedalEfeito",
                        Dimensoes = new RequestDimensaoDto { Altura = 10, Largura = 15, Comprimento = 8 }
                    },
                    new RequestProdutoDto
                    {
                        ProdutoId = "FontePedal",
                        Dimensoes = new RequestDimensaoDto { Altura = 5, Largura = 5, Comprimento = 10 }
                    }
                }
            };

            // Act
            var resultado = _estrategia.OrganizarProdutosEmCaixas(pedido, _caixasDisponiveisPadrao);

            // Assert
            resultado.Should().HaveCount(1);
            var caixaUnica = resultado.First();
            caixaUnica.CaixaId.Should().Be("Caixa 1");
            caixaUnica.Produtos.Should().HaveCount(3)
                .And.Contain("EquipamentoAudioGrande")
                .And.Contain("PedalEfeito")
                .And.Contain("FontePedal");
        }

        [Fact]
        public void Deve_UsarCaixaTresParaItemMuitoGrandeUmPequeno()
        {
            // Arr
            var pedido = new RequestPedidoDto
            {
                PedidoId = 23,
                Produtos = new List<RequestProdutoDto>
                {
                    new RequestProdutoDto
                    {
                        ProdutoId = "EsculturaGrande",
                        Dimensoes = new RequestDimensaoDto { Altura = 45, Largura = 75, Comprimento = 55 }
                    },
                    new RequestProdutoDto
                    {
                        ProdutoId = "Miniatura",
                        Dimensoes = new RequestDimensaoDto { Altura = 8, Largura = 5, Comprimento = 4 }
                    }
                }
            };

            // Act
            var resultado = _estrategia.OrganizarProdutosEmCaixas(pedido, _caixasDisponiveisPadrao);

            // Assert
            resultado.Should().HaveCount(1);

            var caixa3 = resultado.FirstOrDefault(c => c.CaixaId == "Caixa 3");
            caixa3.Should().NotBeNull();
            caixa3.Produtos.Should().HaveCount(2)
                .And.Contain("EsculturaGrande")
                .And.Contain("Miniatura");
        }
    }
}
