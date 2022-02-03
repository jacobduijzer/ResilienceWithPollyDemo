# Polly

![Context](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/context.pu?token=GHSAT0AAAAAABNV6QMYWQQQ3ISBNNR27G7WYP2QPIQ)

## Ordinary API
No rate limit, no faults.

## Faulty API (Random errors every 0..x requests)

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMYNS3TQ3J4M5RFNURWYP2RMRQ)

### Simple retry policy, retry 3 times, otherwise throw

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMYLOD3BPYVNCBXNCOSYP2RKRA)

## Rate limited API

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMYNS3TQ3J4M5RFNURWYP2RMRQ)

### Retry 3 times, go to alternative fallback flow

![Fallback](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryfallback.pu?token=GHSAT0AAAAAABNV6QMZVWIJ5CRE3JGGRHTWYP2ROVQ)

## Rate limited API with grace period, sliding window

### Simple retry policy, retry forever : never recovers

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMYNS3TQ3J4M5RFNURWYP2RMRQ)

### Retry x times, crashes

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMYLOD3BPYVNCBXNCOSYP2RKRA)

### Policy with circuit breaker: succeeds

## Crashing API (Rate limited API with grace period, sliding window)
* Policy with fallback, save to disk as failed, ready to retry later

### What does the logic of the API look like:

![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/ratelimit.pu?token=GHSAT0AAAAAABNV6QMY5ABL5GQNL5AFZWSQYP4F27A)

