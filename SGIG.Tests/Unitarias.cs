using SGIG.Negocio;

namespace SGIG.Tests;

/// <summary>Pruebas unitarias puras (sin base de datos): validaciones y hash de contraseñas.</summary>
public class ValidacionesTests
{
    [Theory]
    [InlineData("1234567", true)]
    [InlineData("12345678", true)]
    [InlineData("123456", false)]      // muy corto
    [InlineData("123456789", false)]   // muy largo
    [InlineData("12.345.678", false)]  // con puntos
    [InlineData("ABC12345", false)]    // letras
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void Dni_exige_siete_u_ocho_digitos(string documento, bool esperado) =>
        Assert.Equal(esperado, Validaciones.EsDocumentoValido(documento, Validaciones.IdTipoDocumentoDni));

    [Theory]
    [InlineData("AB123456", true)]     // pasaporte alfanumérico
    [InlineData("123456", true)]
    [InlineData("12345", false)]       // < 6
    [InlineData("AB 123456", false)]   // espacio
    public void Documento_generico_es_alfanumerico_de_6_a_20(string documento, bool esperado) =>
        Assert.Equal(esperado, Validaciones.EsDocumentoValido(documento, idTipoDocumento: 2));

    [Theory]
    [InlineData("ana@gmail.com", true)]
    [InlineData("a.b-c@sub.dominio.com.ar", true)]
    [InlineData("sin-arroba.com", false)]
    [InlineData("a@b", false)]
    [InlineData("a b@c.com", false)]
    [InlineData("", false)]
    public void Email_valida_estructura(string email, bool esperado) =>
        Assert.Equal(esperado, Validaciones.EsEmailValido(email));

    [Theory]
    [InlineData("3794123456", true)]
    [InlineData("+54 9 379 412-3456", true)]
    [InlineData("(0379) 4123456", true)]
    [InlineData("12", false)]          // muy corto
    [InlineData("abcdefgh", false)]    // letras
    [InlineData("12345678901234", false)] // > 13 dígitos
    public void Telefono_valida_formato_y_cantidad_de_digitos(string telefono, bool esperado) =>
        Assert.Equal(esperado, Validaciones.EsTelefonoValido(telefono));

    [Fact]
    public void ValidarDatosDePersona_no_exige_email_ni_telefono()
    {
        var ex = Record.Exception(() => Validaciones.ValidarDatosDePersona("12345678", 1, null, null));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidarDatosDePersona_rechaza_con_NegocioException()
    {
        Assert.Throws<NegocioException>(() => Validaciones.ValidarDatosDePersona("xx", 1, null, null));
        Assert.Throws<NegocioException>(() => Validaciones.ValidarDatosDePersona("12345678", 1, "mal", null));
        Assert.Throws<NegocioException>(() => Validaciones.ValidarDatosDePersona("12345678", 1, null, "1"));
    }
}

public class HashTests
{
    [Fact]
    public void Calcular_es_SHA256_de_32_bytes_y_determinista()
    {
        var a = Hash.Calcular("clave");
        Assert.Equal(32, a.Length);
        Assert.Equal(a, Hash.Calcular("clave"));
        Assert.NotEqual(a, Hash.Calcular("clave2"));
    }

    [Fact]
    public void Coincide_distingue_contraseña_correcta_de_incorrecta()
    {
        var h = Hash.Calcular("abc123");
        Assert.True(Hash.Coincide("abc123", h));
        Assert.False(Hash.Coincide("abc124", h));
        Assert.False(Hash.Coincide("ABC123", h));
    }

    [Fact]
    public void Hash_de_admin1234_coincide_con_el_del_seed_SQL() =>
        Assert.Equal("0xAC9689E2272427085E35B9D3E3E8BED88CB3434828B43B86FC0596CAD4C6E270",
            Hash.ATextoHex(Hash.Calcular("admin1234")), ignoreCase: true);
}
