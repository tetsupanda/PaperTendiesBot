# Architecture Plans
Just some random ideas on what this potentially could look like. Mainly just a personal exercise in designing/building/deploying a distributed system.

## Thoughts
- Datastore of some sort for historical performance data; measure of an algo's performance, health, etc.
- Multiple services, maybe one per algorithm? That might be overkill of course
- Likely a standalone service to run/maintain the ML model for market sentiment
- Thinking a kubernetes cluster to run the services
- Targeting Azure personally for the expsoure