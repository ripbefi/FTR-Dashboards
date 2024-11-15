from my.synthesis.apps.logs
where program -> "uat"
where aks_container_name -> "vn-document-scheduler-sub"
, matches(body, "Casrcm54vnService.*batch=[E|D]")
group every 1d
select last(eventdate) as lastEventDate
,last(body) as message
,peek(message, re("(InProgress|Finish)"), 1) as jobstatus
,peek(message, re("batch=(\\w*)"), 1) as batch
,peek(message, re("duration\\(ms\\)=(\\w*)"), 1) as duration
,peek(message, re("times=(\\w*)"), 1) as times
,peek(message, re("totalCasNum=(\\w*)"), 1) as totalCasNum
,peek(message, re("totalDistinctNum=(\\w*)"), 1) as totalDistinctNum
,peek(message, re("duplicated=(\\w*)"), 1) as duplicated
,round((float(totalCasNum)/2797642)*100,2) as progress2
,round(float(duration)/(1000*60*60), 1) as duration2
,'Status: ' + jobstatus + '(' + progress2 + '%)'
 + '<br>' + 'Last Update: ' + formatdate(lastEventDate, 'HH:mm:ss')
 + '<br>'
 + '<br>' + 'Batch: ' + batch
 + '<br>' + 'Duration: ' + duration2 + ' hours'
 + ' / Est. Remaining: ' + round(duration2 * ((100-progress2)/progress2), 1) + ' hours'
 + '<br>' + 'Process: ' + totalCasNum
 + ' / Remaining: ' + (2797642 - int(totalCasNum))
 + '<br>' + 'New: ' + (int(totalDistinctNum) - int(duplicated))
 + ', Update: ' + duplicated
 + ', Ignore: ' + (int(totalCasNum) - int(totalDistinctNum))
 as content

//16:31:52.746 [task-4] INFO c.m.a.service.impl.Casrcm54vnService - Ecard (CASRCM54VN) - InProgress - Handle batch=E20240906 in duration(ms)=11810968 times=547, totalCasNum=1094000, totalDistinctNum=1094000, duplicated=0
