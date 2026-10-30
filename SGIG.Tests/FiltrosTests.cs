using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Barras de filtros de Socios, Usuarios y Máquinas (Fase 14). Unitarias: no usan la base.</summary>
public class FiltrosTests
{
    private static readonly DateTime Hoy = new(2026, 10, 28);

    private static Socio S(string apellido, bool activo = true, int? diasParaVencer = 30, int? idPlan = 1) => new()
    {
        Apellido = apellido,
        Nombre = "N",
        Documento = "D" + apellido,
        Activo = activo,
        IdPlan = idPlan,
        FechaVencimientoCuota = diasParaVencer is int d ? Hoy.AddDays(d) : null
    };

    private static readonly List<Socio> Socios = new()
    {
        S("AlDia", diasParaVencer: 30),
        S("PorVencer", diasParaVencer: 3),
        S("VenceHoy", diasParaVencer: 0),
        S("Vencida", diasParaVencer: -2),
        S("SinCuota", diasParaVencer: null, idPlan: null),
        S("DeBaja", activo: false, idPlan: 2),
    };

    private static string[] Apellidos(IEnumerable<Socio> socios) => socios.Select(s => s.Apellido).ToArray();

    [Theory]
    [InlineData(FiltroCuota.AlDia, new[] { "AlDia", "DeBaja" })]
    [InlineData(FiltroCuota.PorVencer, new[] { "PorVencer", "VenceHoy" })]
    [InlineData(FiltroCuota.Vencida, new[] { "Vencida" })]
    [InlineData(FiltroCuota.SinCuotas, new[] { "SinCuota" })]
    public void Socios_por_situacion_de_la_cuota(FiltroCuota cuota, string[] esperados)
    {
        Assert.Equal(esperados, Apellidos(ServicioSocio.Filtrar(Socios, new FiltroSocios { Cuota = cuota }, Hoy)));
    }

    [Fact]
    public void Socios_por_estado_plan_y_texto_se_combinan()
    {
        Assert.Equal(new[] { "DeBaja" }, Apellidos(ServicioSocio.Filtrar(Socios, new FiltroSocios { Estado = FiltroEstado.DadosDeBaja }, Hoy)));
        Assert.Equal(5, ServicioSocio.Filtrar(Socios, new FiltroSocios { Estado = FiltroEstado.Activos }, Hoy).Count);
        Assert.Equal(new[] { "DeBaja" }, Apellidos(ServicioSocio.Filtrar(Socios, new FiltroSocios { IdPlan = 2 }, Hoy)));
        Assert.Equal(new[] { "Vencida" },
            Apellidos(ServicioSocio.Filtrar(Socios, new FiltroSocios { Texto = "venc", Cuota = FiltroCuota.Vencida }, Hoy)));
        Assert.Equal(Socios.Count, ServicioSocio.Filtrar(Socios, new FiltroSocios(), Hoy).Count);
    }

    [Fact]
    public void Usuarios_por_rol_estado_y_texto()
    {
        var usuarios = new List<Usuario>
        {
            new() { Apellido = "Admin", Nombre = "A", Documento = "1", Legajo = "L1", NombreUsuario = "admin", IdRol = Roles.Administrador },
            new() { Apellido = "Recep", Nombre = "R", Documento = "2", Legajo = "L2", NombreUsuario = "recep", IdRol = Roles.Recepcionista },
            new() { Apellido = "Tecnico", Nombre = "T", Documento = "3", Legajo = "L3", NombreUsuario = "tec", IdRol = Roles.Tecnico, Activo = false },
        };

        Assert.Single(ServicioUsuario.Filtrar(usuarios, new FiltroUsuarios { IdRol = Roles.Recepcionista }));
        Assert.Equal("Tecnico", ServicioUsuario.Filtrar(usuarios, new FiltroUsuarios { Estado = FiltroEstado.DadosDeBaja }).Single().Apellido);
        Assert.Equal("Admin", ServicioUsuario.Filtrar(usuarios, new FiltroUsuarios { Texto = "L1" }).Single().Apellido);
        Assert.Empty(ServicioUsuario.Filtrar(usuarios, new FiltroUsuarios { IdRol = Roles.Tecnico, Estado = FiltroEstado.Activos }));
    }

    [Fact]
    public void Maquinas_por_estado_marca_y_texto()
    {
        var maquinas = new List<Maquina>
        {
            new() { Nombre = "Cinta de correr", Marca = "Life Fitness", Estado = "Operativa" },
            new() { Nombre = "Bicicleta", Marca = "Matrix", Estado = "En Reparacion" },
            new() { Nombre = "Mancuernas", Marca = null, Estado = "Operativa" },
        };

        Assert.Equal("Bicicleta", ServicioMaquina.Filtrar(maquinas, new FiltroMaquinas { Estado = "En Reparacion" }).Single().Nombre);
        Assert.Equal("Cinta de correr", ServicioMaquina.Filtrar(maquinas, new FiltroMaquinas { Marca = "life fitness" }).Single().Nombre);
        Assert.Equal("Bicicleta", ServicioMaquina.Filtrar(maquinas, new FiltroMaquinas { Texto = "matr" }).Single().Nombre);
        Assert.Equal(3, ServicioMaquina.Filtrar(maquinas, new FiltroMaquinas()).Count);
    }

    [Theory]
    [InlineData(null, "—")]
    [InlineData("2026-06-01", "Menos de 1 año")]
    [InlineData("2025-10-28", "1 año")]
    [InlineData("2018-01-01", "8 años")]
    public void Antiguedad_de_una_maquina(string? compra, string esperado)
    {
        Assert.Equal(esperado, ServicioMaquina.Antiguedad(compra is null ? null : DateTime.Parse(compra), Hoy));
    }
}
