@echo off
cls

powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0\tools\Psake\psake.ps1' 'Build.ps1' %1"
