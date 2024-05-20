import pandas as pd

# 엑셀 파일 경로
excel_file = 'C:\libo_liti\Test.xlsx'

# 엑셀 파일 불러오기
excel_data = pd.ExcelFile(excel_file)

# 각 시트를 순회하며 CSV 파일로 저장
for sheet_name in excel_data.sheet_names:
    # 현재 시트의 데이터를 DataFrame으로 불러오기
    df = excel_data.parse(sheet_name)

    # CSV 파일로 저장 (시트 이름을 파일 이름으로 사용)
    csv_file = f'{sheet_name}.csv'
    df.to_csv(csv_file, index=False, encoding='utf-8-sig')
    print(f'{csv_file} 파일이 저장되었습니다.')
