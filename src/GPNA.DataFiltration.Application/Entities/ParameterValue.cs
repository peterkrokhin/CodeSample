using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNA.DataFiltration.App.Entities
{
    public class ParameterValue
    {
        /// <summary>
        /// Идентификатор скважины OIS (WELL_ID)
        /// </summary>
        public long WellId { get; set; }

        /// <summary>
        /// Внешний идентификатор параметра (PARAM_ID)
        /// </summary>
        public int ParameterId { get; set; }

        /// <summary>
        /// Значение технологического параметра (MEASURE_VALUE)
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// Дата изменения технологического параметра, зафиксированная контроллером (MEASURE_DATE)
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Периодичность ведения замеров (MEASURE_DURATION)
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// Тип скважины: нагнетательная/нефтяная/газовая
        /// </summary>
        //public ObjectType WellType { get; set; }
    }
}
