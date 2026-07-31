using System.Collections.Generic;
using NUnit.Framework;

// ============================================================
// 테스트 대상 인터페이스 (실제 구현은 프로젝트의 NicknameService로 교체)
// 팀의 실제 서비스 시그니처에 맞춰 이 부분만 바꿔주면 됩니다.
// ============================================================
public enum NicknameResult
{
    Success,
    TooShort,
    TooLong,
    InvalidCharacter,
    ContainsWhitespaceOnly,
    ContainsForbiddenWord,
    AlreadyTaken,
    Empty
}

public interface INicknameService
{
    NicknameResult Validate(string nickname);
    NicknameResult SetNickname(string characterId, string nickname);
}

// ============================================================
// 테스트 케이스
// ============================================================
[TestFixture]
public class NicknameValidationTests
{
    private INicknameService _service;

    [SetUp]
    public void SetUp()
    {
        // 실제 구현체 또는 Mock으로 교체
        _service = new FakeNicknameService();
    }

    // ---------------------------------------------------------
    // 1. 길이 검증 (2~12자 가정)
    // ---------------------------------------------------------
    [Test]
    public void Validate_길이가_최소값_미만이면_TooShort를_반환한다()
    {
        var result = _service.Validate("가"); // 1자
        Assert.AreEqual(NicknameResult.TooShort, result);
    }

    [Test]
    public void Validate_길이가_최소값이면_통과한다()
    {
        var result = _service.Validate("가나"); // 2자 (경계값)
        Assert.AreEqual(NicknameResult.Success, result);
    }

    [Test]
    public void Validate_길이가_최대값이면_통과한다()
    {
        var result = _service.Validate("가나다라마바사아자차카"); // 12자 (경계값)
        Assert.AreEqual(NicknameResult.Success, result);
    }

    [Test]
    public void Validate_길이가_최대값을_초과하면_TooLong을_반환한다()
    {
        var result = _service.Validate("가나다라마바사아자차카타"); // 13자
        Assert.AreEqual(NicknameResult.TooLong, result);
    }

    // ---------------------------------------------------------
    // 2. 빈 값 / 공백 처리
    // ---------------------------------------------------------
    [Test]
    public void Validate_빈문자열이면_Empty를_반환한다()
    {
        var result = _service.Validate("");
        Assert.AreEqual(NicknameResult.Empty, result);
    }

    [Test]
    public void Validate_null이면_Empty를_반환한다()
    {
        var result = _service.Validate(null);
        Assert.AreEqual(NicknameResult.Empty, result);
    }

    [Test]
    public void Validate_공백만_입력하면_ContainsWhitespaceOnly를_반환한다()
    {
        var result = _service.Validate("    ");
        Assert.AreEqual(NicknameResult.ContainsWhitespaceOnly, result);
    }

    [Test]
    public void Validate_앞뒤공백은_trim되어_검증된다()
    {
        var result = _service.Validate("  홍길동  ");
        Assert.AreEqual(NicknameResult.Success, result);
    }

    [Test]
    public void Validate_단어_중간에_공백이_있으면_InvalidCharacter를_반환한다()
    {
        var result = _service.Validate("홍 길동");
        Assert.AreEqual(NicknameResult.InvalidCharacter, result);
    }

    // ---------------------------------------------------------
    // 3. 허용 문자 (한글/영문/숫자만 허용 가정)
    // ---------------------------------------------------------
    [TestCase("hero123")]
    [TestCase("용사123")]
    [TestCase("Player1")]
    public void Validate_허용된_문자조합이면_통과한다(string nickname)
    {
        var result = _service.Validate(nickname);
        Assert.AreEqual(NicknameResult.Success, result);
    }

    [TestCase("hero!")]
    [TestCase("용사@")]
    [TestCase("player#1")]
    [TestCase("😀용사")] // 이모지
    [TestCase("hero\n")] // 제어문자
    public void Validate_허용되지않은_특수문자가_포함되면_InvalidCharacter를_반환한다(string nickname)
    {
        var result = _service.Validate(nickname);
        Assert.AreEqual(NicknameResult.InvalidCharacter, result);
    }

    // ---------------------------------------------------------
    // 4. 금칙어 필터
    // ---------------------------------------------------------
    [Test]
    public void Validate_금칙어가_포함되면_ContainsForbiddenWord를_반환한다()
    {
        var result = _service.Validate("운영자김철수"); // 서비스 내 금칙어 목록에 "운영자" 포함 가정
        Assert.AreEqual(NicknameResult.ContainsForbiddenWord, result);
    }

    [Test]
    public void Validate_금칙어가_대소문자만_다르게_섞여도_걸러진다()
    {
        var result = _service.Validate("GM관리자"); // 대소문자 무시 검사 가정
        Assert.AreEqual(NicknameResult.ContainsForbiddenWord, result);
    }

    // ---------------------------------------------------------
    // 5. 중복 닉네임 (SetNickname 레벨에서 검증)
    // ---------------------------------------------------------
    [Test]
    public void SetNickname_이미_사용중인_닉네임이면_AlreadyTaken을_반환한다()
    {
        _service.SetNickname("char_001", "용사");
        var result = _service.SetNickname("char_002", "용사");

        Assert.AreEqual(NicknameResult.AlreadyTaken, result);
    }

    [Test]
    public void SetNickname_본인이_기존과_동일한_닉네임으로_재설정해도_AlreadyTaken이_아니다()
    {
        _service.SetNickname("char_001", "용사");
        var result = _service.SetNickname("char_001", "용사");

        Assert.AreEqual(NicknameResult.Success, result);
    }

    [Test]
    public void SetNickname_대소문자만_다른_닉네임은_중복으로_처리된다()
    {
        _service.SetNickname("char_001", "Hero");
        var result = _service.SetNickname("char_002", "hero");

        Assert.AreEqual(NicknameResult.AlreadyTaken, result);
    }

    [Test]
    public void SetNickname_검증에_실패하면_중복체크_전에_실패를_반환한다()
    {
        // 유효성 검증(길이/문자) 실패가 중복체크보다 먼저 처리되는지 확인
        var result = _service.SetNickname("char_003", "a");
        Assert.AreEqual(NicknameResult.TooShort, result);
    }
}

// ============================================================
// 테스트용 Fake 구현 (실제 서비스 연결 전까지 임시로 사용)
// 실제 프로젝트에서는 이 클래스를 지우고 실서비스/Mock 라이브러리(Moq 등)로 교체하세요.
// ============================================================
internal class FakeNicknameService : INicknameService
{
    private static readonly HashSet<string> ForbiddenWords = new HashSet<string> { "운영자", "gm", "admin" };
    private readonly Dictionary<string, string> _charToNickname = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _nicknameOwner = new Dictionary<string, string>(); // key: lower nickname

    public NicknameResult Validate(string nickname)
    {
        if (string.IsNullOrEmpty(nickname)) return NicknameResult.Empty;

        var trimmed = nickname.Trim();
        if (trimmed.Length == 0) return NicknameResult.ContainsWhitespaceOnly;

        if (trimmed.Length < 2) return NicknameResult.TooShort;
        if (trimmed.Length > 12) return NicknameResult.TooLong;

        foreach (var c in trimmed)
        {
            bool isKorean = c >= 0xAC00 && c <= 0xD7A3;
            bool isAlpha = char.IsLetterOrDigit(c) && c < 128;
            if (!isKorean && !isAlpha) return NicknameResult.InvalidCharacter;
        }

        var lower = trimmed.ToLowerInvariant();
        foreach (var word in ForbiddenWords)
        {
            if (lower.Contains(word)) return NicknameResult.ContainsForbiddenWord;
        }

        return NicknameResult.Success;
    }

    public NicknameResult SetNickname(string characterId, string nickname)
    {
        var validation = Validate(nickname);
        if (validation != NicknameResult.Success) return validation;

        var trimmed = nickname.Trim();
        var key = trimmed.ToLowerInvariant();

        if (_nicknameOwner.TryGetValue(key, out var owner) && owner != characterId)
        {
            return NicknameResult.AlreadyTaken;
        }

        if (_charToNickname.TryGetValue(characterId, out var prevNickname))
        {
            _nicknameOwner.Remove(prevNickname.ToLowerInvariant());
        }

        _charToNickname[characterId] = trimmed;
        _nicknameOwner[key] = characterId;
        return NicknameResult.Success;
    }
}
