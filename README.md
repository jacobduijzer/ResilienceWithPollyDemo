# Resilience with Polly

![Context](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/context.pu?token=GHSAT0AAAAAABNV6QMYF2TBW777YWLEET2YYQAZVOA)

## Getting Started

To test the code and the policies start the `Buggy API` in the `externalapi` folder:

``` docker
docker-compose up
```

## 1. Ordinary API
No rate limit, no faults.

## 2. Faulty API (Random errors every 0..x requests)

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Simple retry policy, retry 3 times, otherwise throw

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMYVDN6ZX6PDGYEWELSYQAZV4Q)

## 3. Rate limited API

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Retry 3 times, go to alternative fallback flow

![Fallback](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryfallback.pu?token=GHSAT0AAAAAABNV6QMZR6T7JNTW5JDD5WLUYQAZWSA)

## 4. Rate limited API with grace period, sliding window

### Simple retry policy, retry forever : never recovers

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Retry x times, crashes

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMYVDN6ZX6PDGYEWELSYQAZV4Q)

### Policy with circuit breaker: succeeds

## Crashing API (Rate limited API with grace period, sliding window)
* Policy with retry and circuit breaker

### What does the logic of the API look like:

![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/ratelimit.pu?token=GHSAT0AAAAAABNV6QMYO43FJTR6B5O6GW5CYQAZXGQ)

### What does the retry policy look like:
![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/circuitbreaker.pu?token=GHSAT0AAAAAABNV6QMZIPZOG56BZGP2NEG4YQAZXSA)
