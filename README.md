# Path-of-the-Legacy
|<img src="https://github.com/user-attachments/assets/b8effee5-a594-419b-b65c-a92d83bbb459" width="50%" height="50%">| <img src="https://github.com/user-attachments/assets/755bae6f-3436-4f0b-808a-40cf284d650e" width="50%" height="50%">

## 게임 소개
- 장르 : 3D MMORPG

- 개발기간 : 2024.12.01 -> 2025.02.15

- 목적  : 3d RPG를 만들어보고자 만들어본 프로젝트입니다.
  
- 관리 : Github/Jira

## 개발 환경
- 플랫폼 : Windows 11

- 언어 : C#

- 엔진 환경 : Unity 2022.03.15(LTS)

## 구현 기능
* UI
  * Scene
      * InGame :HUD (HP,Mp,경험치바,미니맵)
      * Menu : 메뉴창(나가기,설정,일시정지)
  * Popup
      * 퀘스트 팝업창
      * 장비,스탯창
      * 인벤토리창
      * 대화창
  * World
      * 몬스터 HP바,닉네임,퀘스트 마크


## 사용 기술

| 항목 | 설명 |
| ------------ | ------------- |
| 디자인 패턴 | 싱글톤 패턴을 사용해서 전역 접근 관리 & State Pattern을 사용해 캐릭터 애니메이션을 객체 관리|
| Save | 게임내 데이터를 글로벌 변수에 저장 및 관리 |
| SkinnedMesh| 장비 장착시 캐릭터의 의상이 변경되도록 구현|
| 상속 | 상속을 통해 같은 패턴을 쓰는 캐릭터를 통합 관리|

## 기술 문서
[기술 문서](https://docs.google.com/presentation/d/17gUVNPHz_Csg8ZsBX49GK2CNHrIdn4ZIBZpw9Fcrn50/edit?usp=sharing)

 ## velog

[블로그](https://velog.io/@typhoon760/posts?tag=%ED%8F%AC%ED%8A%B8%ED%8F%B4%EB%A6%AC%EC%98%A4)

## 영상
[플레이 영상](https://youtu.be/0b5-uaT7wGA)
