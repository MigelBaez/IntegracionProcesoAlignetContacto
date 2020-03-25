using Cliente360.Integracion.AlignetContacto.Entities;
using Release.Helper.Data.ICore;
using System.Collections.Generic;

namespace Cliente360.Integracion.AlignetContacto.Core
{
    public interface IUnitOfWork : IBaseUnitOfWorkAdo
    {
        IEnumerable<BaseTransacciones> ObtenerTransaccionesSinContacto();
        void AddTransacciones(List<BaseTransacciones> baseAlignet, List<BaseAsl> baseAslContacto);

    }
}
