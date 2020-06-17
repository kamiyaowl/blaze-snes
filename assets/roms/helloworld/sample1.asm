;----------------------------------------------------------------------------
;			SNES Startup Routine
;					Copyright (C) 2007, Tekepen
;----------------------------------------------------------------------------
.setcpu		"65816"
.autoimport	on

.import	InitRegs

.segment "STARTUP"

; パレットテーブル
Palette:
	.incbin	"palette.bin"

; パターンテーブル
Pattern:
	.incbin	"moji.chr"

; 文字列テーブル
String:
	.asciiz	"HELLO, WORLD!"

; リセット割り込み
.proc	Reset
	sei
	clc
	xce	; Native Mode
	phk
	plb	; DB = 0

	rep	#$30	; A,I 16bit
.a16
.i16
	ldx	#$1fff
	txs
	
	jsr	InitRegs

	sep	#$20
.a8
	lda	#$40
	sta	$2107
	stz	$210b
	
; Copy Palettes
	stz	$2121
	ldy	#$0200
	ldx	#$0000
copypal:
	lda	Palette, x
	sta	$2122
	inx
	dey
	bne	copypal

; Copy Patterns
	rep	#$20
.a16
	lda	#$0000
	sta	$2116
	ldy	#$2000
	ldx	#$0000
copyptn:
	lda	Pattern, x
	sta	$2118
	inx
	inx
	dey
	bne	copyptn

; Copy NameTable
	lda	#$41a9
	sta	$2116
	ldy	#$000d
	ldx	#$0000
	lda	#$0000
copyname:
	sep	#$20
.a8
	lda	String, x
	rep	#$20
.a16
	sta	$2118
	inx
	dey
	bne	copyname

	lda	#$01
	sta	$212c
	stz	$212d
	lda	#$0f
	sta	$2100
mainloop:
	jmp	mainloop

	rti
.endproc

; カートリッジ情報
.segment "CARTINFO"
	.byte	"SAMPLE1               "	; Game Title
	.byte	$01				; 0x01:HiRom, 0x30:FastRom(3.57MHz)
	.byte	$05				; ROM Size (2KByte * N)
	.byte	$00				; RAM Size (8KByte * N)
	.word	$0001				; Developper ID ?
	.byte	$00				; Version
	.byte	$7f, $73, $80, $8c		; Security Key ?
	.byte	$ff, $ff, $ff, $ff		; Security Key ?

	.word	$0000	; Native:COP
	.word	$0000	; Native:BRK
	.word	$0000	; Native:ABORT
	.word	$0000	; Native:NMI
	.word	$0000	; 
	.word	$0000	; Native:IRQ

	.word	$0000	; 
	.word	$0000	; 

	.word	$0000	; Emulation:COP
	.word	$0000	; 
	.word	$0000	; Emulation:ABORT
	.word	$0000	; Emulation:NMI
	.word	Reset	; Emulation:RESET
	.word	$0000	; Emulation:IRQ/BRK
