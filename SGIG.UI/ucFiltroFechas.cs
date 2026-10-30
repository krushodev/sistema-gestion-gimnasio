// Control armado sólo por código (nunca se arrastra al diseñador).
#pragma warning disable WFO1000
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI
{
    /// <summary>
    /// Filtro de fechas reutilizable (Fase 14) para Gráficos y Reportes: un combo de período
    /// rápido y Desde/Hasta. Elegir un período carga las fechas; tocar una fecha a mano pasa
    /// el combo a "Personalizado". En ambos casos avisa con <see cref="RangoCambiado"/>.
    /// </summary>
    //
    // ── CONTROLES (creados por código) ───────────────────────────────────────
    //   lblPeriodo, cboPeriodo, lblDesde, dtpDesde, lblHasta, dtpHasta
    // ───────────────────────────────────────────────────────────────────────────
    public class ucFiltroFechas : UserControl
    {
        public enum Periodo { Hoy, Ultimos7Dias, EsteMes, MesAnterior, Ultimos6Meses, Ultimos12Meses, EsteAnio, Personalizado }

        private static readonly string[] Nombres =
        {
            "Hoy", "Últimos 7 días", "Este mes", "Mes anterior",
            "Últimos 6 meses", "Últimos 12 meses", "Este año", "Personalizado"
        };

        private readonly ComboBox cboPeriodo = new() { Name = "cboPeriodo", DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
        private readonly DateTimePicker dtpDesde = new() { Name = "dtpDesde", Format = DateTimePickerFormat.Short, Width = 120 };
        private readonly DateTimePicker dtpHasta = new() { Name = "dtpHasta", Format = DateTimePickerFormat.Short, Width = 120 };
        private bool _cargandoPeriodo;

        /// <summary>Se dispara cuando cambia el rango (por período rápido o a mano).</summary>
        public event EventHandler? RangoCambiado;

        public DateTime Desde => dtpDesde.Value.Date;
        public DateTime Hasta => dtpHasta.Value.Date;

        /// <summary>"del 01/10/2026 al 28/10/2026" (o "del 28/10/2026" si es un solo día).</summary>
        public string Descripcion => Desde == Hasta
            ? $"del {Desde:dd/MM/yyyy}"
            : $"del {Desde:dd/MM/yyyy} al {Hasta:dd/MM/yyyy}";

        public ucFiltroFechas(Periodo inicial)
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Tema.Fondo;

            var fila = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                BackColor = Tema.Fondo,
                Margin = new Padding(0)
            };
            cboPeriodo.Items.AddRange(Nombres);
            dtpDesde.Margin = dtpHasta.Margin = cboPeriodo.Margin = new Padding(0, 4, 0, 0);
            fila.Controls.AddRange(new Control[]
            {
                Rotulo("Período", 0), cboPeriodo,
                Rotulo("Desde", 16), dtpDesde,
                Rotulo("Hasta", 12), dtpHasta
            });
            Controls.Add(fila);

            CargarPeriodo(inicial);
            cboPeriodo.SelectedIndexChanged += (s, e) =>
            {
                if (_cargandoPeriodo || cboPeriodo.SelectedIndex == (int)Periodo.Personalizado) return;
                CargarPeriodo((Periodo)cboPeriodo.SelectedIndex);
                RangoCambiado?.Invoke(this, EventArgs.Empty);
            };
            dtpDesde.ValueChanged += (s, e) => FechaCambiadaAMano();
            dtpHasta.ValueChanged += (s, e) => FechaCambiadaAMano();
        }

        private static Label Rotulo(string texto, int margenIzquierdo) => new()
        {
            Text = texto,
            AutoSize = true,
            Font = Tema.Fuente(9F, FontStyle.Bold),
            ForeColor = Tema.TextoSecundario,
            Margin = new Padding(margenIzquierdo, 9, 6, 0)
        };

        private void CargarPeriodo(Periodo periodo)
        {
            _cargandoPeriodo = true;
            var (desde, hasta) = Rango(periodo, DateTime.Today);
            // Se asigna primero el extremo que no deja el rango invertido a mitad de camino.
            if (desde > dtpHasta.Value) { dtpHasta.Value = hasta; dtpDesde.Value = desde; }
            else { dtpDesde.Value = desde; dtpHasta.Value = hasta; }
            cboPeriodo.SelectedIndex = (int)periodo;
            _cargandoPeriodo = false;
        }

        private void FechaCambiadaAMano()
        {
            if (_cargandoPeriodo) return;
            _cargandoPeriodo = true;
            cboPeriodo.SelectedIndex = (int)Periodo.Personalizado;
            _cargandoPeriodo = false;
            RangoCambiado?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Fechas (inclusive) de cada período rápido, tomando <paramref name="hoy"/> como referencia.</summary>
        public static (DateTime Desde, DateTime Hasta) Rango(Periodo periodo, DateTime hoy)
        {
            hoy = hoy.Date;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            return periodo switch
            {
                Periodo.Hoy => (hoy, hoy),
                Periodo.Ultimos7Dias => (hoy.AddDays(-6), hoy),
                Periodo.EsteMes => (inicioMes, hoy),
                Periodo.MesAnterior => (inicioMes.AddMonths(-1), inicioMes.AddDays(-1)),
                Periodo.Ultimos6Meses => (inicioMes.AddMonths(-5), hoy),
                Periodo.Ultimos12Meses => (inicioMes.AddMonths(-11), hoy),
                Periodo.EsteAnio => (new DateTime(hoy.Year, 1, 1), hoy),
                _ => (hoy, hoy)
            };
        }
    }
}
