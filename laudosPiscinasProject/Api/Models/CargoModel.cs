using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class CargoModel
    {
        public static List<CargoViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<cargo> query = context.Set<cargo>();

            List<CargoViewModel> Lista = new List<CargoViewModel>();
            query.ToList().ForEach(car => Lista.Add(new CargoViewModel
            {
                Id = car.car_id_cargo,
                Nome = car.car_ds_cargo.ToUpper()

            }));

            return Lista.OrderBy(x => x.Nome).ToList();
        }

        public class CargoViewModel
        {
            public long Id { get; set; }
            public string Nome { get; set; }

        }

    }
}