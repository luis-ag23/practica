using Microsoft.AspNetCore.Mvc;
using System.Reflection;
namespace practica.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class PaymentsController : ControllerBase
    {
        private static readonly List<Payment> _payments = new()
        {
            new Payment {Id=Guid.NewGuid(),ReservationId=Guid.NewGuid(),Amount=232,PaymentDate=DateTime.Parse("12-12-2023"),PaymentStatus= "paid" },
            new Payment {Id=Guid.NewGuid(),ReservationId=Guid.NewGuid(),Amount=332,PaymentDate=DateTime.Parse("10-11-2024"),PaymentStatus= "pending" },
            new Payment {Id=Guid.NewGuid(),ReservationId=Guid.NewGuid(),Amount=432,PaymentDate=DateTime.Parse("11-10-2025"),PaymentStatus= "pending" },
            new Payment {Id=Guid.NewGuid(),ReservationId=Guid.NewGuid(),Amount=532,PaymentDate=DateTime.Parse("12-9-2026"),PaymentStatus= "failed" },
            new Payment {Id=Guid.NewGuid(),ReservationId=Guid.NewGuid(),Amount=632,PaymentDate=DateTime.Parse("11-8-2027"),PaymentStatus= "paid" },
        };
        [HttpGet("{id:guid}")]
        public IActionResult GetOne(Guid id)
        {
            var payment = _payments.FirstOrDefault(x => x.Id == id);
            return payment is null ?
                NotFound(new {error="Payment not fount",status=404})
                :Ok(payment);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var payment = new Payment
            {
                Id=Guid.NewGuid(),
                ReservationId=Guid.NewGuid(),
                Amount=dto.Amount,
                PaymentDate=dto.PaymentDate,
                PaymentStatus=dto.PaymentStatus.Trim()
            };
            _payments.Add(payment);
            return CreatedAtAction(nameof(GetOne),new {id = payment.Id},payment);
        }
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdatePaymentDto dto)
        {
            if(!ModelState.IsValid) return ValidationProblem(ModelState);
            var index = _payments.FindIndex(x => x.Id == id);
            if (index == -1)
                return NotFound(new { error = "Payment not found", status = 404 });
            var updated = new Payment
            {
                Id =id,
                ReservationId = Guid.NewGuid(),
                Amount=dto.Amount,
                PaymentDate=dto.PaymentDate,
                PaymentStatus=dto.PaymentStatus.Trim()
            };
            _payments[index] = updated;
            return Ok(updated);
        }
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var removed = _payments.RemoveAll(x => x.Id == id);
            return removed == 0 ? NotFound(new { error = "Payment not found", status = 404 })
                : NoContent();
        }
        private static (int page,int limit) NormalizePage(int? page,int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if (l < 1) l = 1; if(l>100) l = 100;
            return (p,l);
        }
        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src,string? sort,string? order)
        {
            if (string.IsNullOrWhiteSpace(sort)) return src;
            var prop = typeof(T).GetProperty(sort,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return src;
            return string.Equals(order,"desc",StringComparison.OrdinalIgnoreCase) 
                ? src.OrderByDescending(x=>prop.GetValue(x))
                : src.OrderBy(x=>prop.GetValue(x));
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery] int? page,
                                    [FromQuery] int? limit,
                                    [FromQuery] string? sort,
                                    [FromQuery] string? order,
                                    [FromQuery] string? q)
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Payment> query = _payments;
            if(!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x =>
                x.PaymentStatus.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            query = OrderByProp(query, sort, order);
            var total = query.Count();
            var data = query.Skip((p - 1) * l).Take(l).ToList();
            return Ok(new
            {
                data,
                meta = new { page = p, limitt = l, total }
            });
        }
    }
}
