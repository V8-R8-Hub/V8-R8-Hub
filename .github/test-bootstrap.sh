#!/usr/bin/env bash
echo postgres | ./DbUpgrader/bin/Debug/net7.0/DbUpgrader bootstrap localhost v8r8hub --user postgres

