# Resilience with Polly

![Context](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/context.pu?token=GHSAT0AAAAAABNV6QMYF2TBW777YWLEET2YYQAZVOA)

## Getting Started

To test the code and the policies start the `Buggy API` in the `externalapi` folder:

``` docker
docker-compose up
```

Make sure all `feature flags` are turned off in the `.env` file:

```
EnableFaultyApi: false
EnableRateLimit: false
EnableCoolingDown: false
```

## 1. Ordinary API
No rate limit, no faults.

## 2. Faulty API (Random errors every 0..x requests)

1. Stop the buggy api
2. Set the feature flag for EnableFaultyApi to true
3. Start the api again: `docker-compose up`

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Simple retry policy, retry 3 times, otherwise throw

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu?token=GHSAT0AAAAAABNV6QMYVDN6ZX6PDGYEWELSYQAZV4Q)

### Retry 3 times, go to alternative fallback flow

![Fallback](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryfallback.pu?token=GHSAT0AAAAAABNV6QMZVWIJ5CRE3JGGRHTWYP2ROVQ)

## 3. Rate limited API

1. Stop the buggy api
2. Set the feature flag for EnableFaultyApi to false
3. Set the feature flag for EnableRateLimit to true
4. Start the api again: `docker-compose up`

### Simple retry policy, retry forever

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu?token=GHSAT0AAAAAABNV6QMZZWJ35WVQXWC7CHL6YQAZUEA)

### Retry 3 times, go to alternative fallback flow

![Fallback](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryfallback.pu?token=GHSAT0AAAAAABNV6QMZR6T7JNTW5JDD5WLUYQAZWSA)

## 4. Rate limited API with grace period, sliding window

1. Stop the buggy api
2. Set the feature flag for EnableFaultyApi to false
3. Set the feature flag for EnableRateLimit to true
4. Set the feature flag for EnableCoolingDown to true
5. Start the api again: `docker-compose up`

### What does the logic of the API look like:

![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/ratelimit.pu)

### What does the retry policy look like:
![API Logic](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/circuitbreaker.pu)


### Simple retry policy, retry forever : never recovers

![Retry forever](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retryforever.pu)

### Retry x times, crashes

![Retry throw](http://www.plantuml.com/plantuml/proxy?src=https://raw.githubusercontent.com/jacobduijzer/ResilienceWithPollyDemo/main/design/retrythrow.pu)

### Policy with circuit breaker: succeeds

## Crashing API (Rate limited API with grace period, sliding window)
* Policy with retry and circuit breaker

