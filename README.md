# Path-of-the-Legacy
<table>
  <tr>
    <td><img src="" width="600"></td>
      <td><img src="" width="600"></td>

  </tr>
  <tr>
    <td><img src="" width="600"></td>
    <td><img src="" width="600"></td>
  </tr>
</table>


## 게임 소개

- 장르 : 3D MMORPG

- 개발기간 : 2025.06.04 -> 2025.09.07

- 목적  : 3d RPG를 구현해보고자 만들어본 프로젝트입니다.
  
- 관리 : Github/Jira

## 개발 환경
- 플랫폼 : Windows 11

- 언어 : C#

- 엔진 환경 : Unity 2022.03.15(LTS)

## 구현 기능
*Object
 * 플레이어
 * 일반 몬스터:
   * 스켈레톤
   * 강화 스켈레톤
   * 엘리트 스켈레톤
* 보스 몬스터 :
  * 불의 정령 ( 패턴 : 기본 공격 2개, 스킬 2개 )
* NPC :
  * Shop NPC (포션, 장신구, 방어구, 무기)
  * Upgrade NPC
  * Quest NPC
* 아이템
  * HP 회복 물약, MP 회복 물약
  * 무기, 방어구, 장신구

* UI
  * Scene :
      * PlayScene : 게임 진행 시 사용 ( 플레이어 스탯, 미니맵, 퀘스트 알림, 스킬 퀵슬롯, 소비 아이템 퀵슬롯 )
      * CustomScene : 캐릭터 커스텀 시 사용 ( 커스텀 부위 변경 버튼, 확인 버튼, 나가기 버튼 )
      * TitleScene : 게임 접속 시 사용 ( 게임 시작 버튼, 세이브 로드 버튼, 나가기 버튼 )
  * Popup
      * 인벤토리창, 장비창, 스킬창, 퀘스트창, 상점창, 강화창, 대화창, 메뉴창
      * 확인창, 개수 입력창, 메뉴창, 부활창, 슬롯Tip창, Scene Load창
     
  * World
      * 피격 데미지 Effect, 체력 Bar, 이름 Bar, 네비게이션, 퀘스트 Icon


## 사용 기술

| 항목 | 설명 |
| ------------ | ------------- |
| 디자인 패턴 | 싱글톤 패턴을 사용하여 Manager 통합 관리 & State 패턴을 사용하여 캐릭터의 기능을 직관적으로 관리|
| Google Sheet | 구글 스프레드 시트를 사용해 데이터 관리|
| Save | 게임 데이터를 모두 Json으로 변환하여 관리|
| ObjectPooling | 자주 사용되는 객체는 Pool로 관리하여 재사용 |
| SkinnedMesh| 캐릭터의 얼굴을 커스텀하고, 장비 장착 시 의상 변경 가능|
| UI 자동화 | 유니티 UI 상에서 컴포넌트로 Drag&Drop의 실수를 줄이기위한 편의 기능|

## GoogleSpreadSheet
[스프레드시트](https://docs.google.com/spreadsheets/d/1JrR2gxJniIMkQcb9BAhsMUXlrcaJXhBNavmh5Zs4IpY/edit?usp=sharing)


## 기술 문서
[기술 문서](https://docs.google.com/presentation/d/1x-NVuzHHo0Xo09vtEFiucUgF4Dag3MydWzX7uFv4jh8/edit?usp=sharing)

 ## velog
[블로그](https://velog.io/@typhoon760/3DRPG%ED%94%84%EB%A1%9C%EC%A0%9D%ED%8A%B8-1)

## 영상
[플레이 영상](https://youtu.be/vQypS2yFtt8)
