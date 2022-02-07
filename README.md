# Polly

![Context](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/context.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

## Ordinary API
No rate limit, no faults.

## Faulty API (Random errors every 0..x requests)

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Simple retry policy, retry 3 times, otherwise throw

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

## Rate limited API

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Retry 3 times, go to alternative fallback flow

![Fallback](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryfallback.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

## Rate limited API with grace period, sliding window

### Simple retry policy, retry forever : never recovers

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Retry x times, crashes

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Policy with circuit breaker: succeeds

## Crashing API (Rate limited API with grace period, sliding window)
* Policy with retry and circuit breaker

### What does the logic of the API look like:

![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/ratelimit.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### What does the retry policy look like:
![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/circuitbreaker.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)
