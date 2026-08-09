select
  cnt.Id as [Контрагент],
  cnt.Name as [Наименование контрагента],
  deal.Id as [Сделка],
  sType.Name as [Этап],
  case stage.IsMain when 1 then 'Основной' else 'Второстепенный' end as [Тип этапа],
  stage.Priority as [Приоритет],
  stage.Status as [Статус],
  stage.StartDate as [Дата начала],
  stage.EndDate as [Дата окончания] 
from
  dbo.Counterparty cnt
  join dbo.Deal deal
    on cnt.Id = deal.CounterpartyId
  join dbo.Stage stage
    on deal.Id = stage.DealId
  join dbo.StageType sType
    on stage.StageType = sType.Id
where
  --cnt.id = 123
  cnt.Name = 'Василий Петров'
order by
  deal.Id,
  stage.Id,
  stage.IsMain desc