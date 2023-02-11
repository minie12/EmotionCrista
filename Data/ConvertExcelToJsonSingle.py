import pandas
import json
import os
import sys

def main(argv):
    if(len(argv) < 2):
        print("Error: Wrong command format. \n...Command format should be: python <__.py> <__.xlsx> <sheetname>")
        return
    
    currentDir = os.getcwd()
    fileName = argv[0]
    excelPath = os.path.join(currentDir, fileName)

    # Read data from excel file
    sheetName = argv[1]
    excelData = pandas.read_excel(excelPath, sheet_name=sheetName)

    # Change excel data to Json and Write to file
    outputName = "FullDialog.json"
    jsonPath = os.path.join(currentDir, outputName)

    jsonData = excelData.to_json(jsonPath, orient='records', force_ascii = False)

    print("Changed excel to json file successfully.")

    
if __name__ == "__main__":
    main(sys.argv[1:])
