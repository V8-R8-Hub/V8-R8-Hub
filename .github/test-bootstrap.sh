#!/usr/bin/env bash
echo postgres | ./DbUpgrader/bin/Debug/net7.0/DbUpgrader.exe bootstrap localhost v8r8hub_test --user postgres

