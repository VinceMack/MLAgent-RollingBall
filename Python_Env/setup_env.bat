@echo off
conda create -n mlagents python=3.10.12 -y
call conda activate mlagents
pip install -r requirements.txt
echo Environment Setup Complete.
pause