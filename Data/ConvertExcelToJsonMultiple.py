import pandas
import json
import os
import sys

currentDir = os.getcwd()
fileName = ["Dialogue.xlsx"]
sheetName = [["D01_Naria", "D01_Lulian", "D01_Russel", "D01_Nish", "D01_Ilrak", "Ending0"]]

excelData = pandas.DataFrame(columns = ["id", "character", "dialog"])

for i in range(len(fileName)):
    excelPath = os.path.join(currentDir, fileName[i])
    
    for j in range(len(sheetName[i])):
        # Read data from excel file
        excelData = pandas.concat([excelData, pandas.read_excel(excelPath, sheet_name=sheetName[i][j])], ignore_index = True)

# Change excel data to Json and Write to file
outputName = "FullDialog.json"
jsonPath = os.path.join(currentDir, outputName)
jsonData = excelData.to_json(jsonPath, orient='records', force_ascii = False)

print("Changed excel to json file successfully.")
